using System.Security.Claims;
using Core;
using Core.Data;
using Core.Messages;
using Core.Models;
using Core.Web;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UsersMicro.Models;
using UsersMicro.Services;
using WebCommons;

namespace UsersMicro.Consumers;

public class EditUserConsumer : TransactionConsumer<EditUserOrder, EditUserResponse>
{
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IRepository<User> users;
	private readonly IRepository<Role> roles;
	private readonly IPasswordService passwordService;
	private User editedUser = default!;
	
	public EditUserConsumer(IHttpContextAccessor httpContextAccessor, ILogger<EditUserConsumer> logger, IRepository<User> users, IPasswordService passwordService,
		IRepository<Role> roles, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.roles = roles;
		this.users = users;
		this.passwordService = passwordService;
		this.httpContextAccessor = httpContextAccessor;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<EditUserOrder> context)
	{
		if (!await users.GetAll().AnyAsync(x => x.Id == context.Message.Id))
		{
			await RespondWithValidationFailAsync(context, "Username", "Nie znaleziono użytkownika");
			return false;
		}
		
		var isAdmin = httpContextAccessor.HasAnyRole(RoleEnum.Administrator);
		if (!isAdmin && httpContextAccessor.GetUserId() == context.Message.Id)
		{
			await RespondWithValidationFailAsync(context, "Username", "Tylko administrator może edytować swoje konto");
			return false;
		}
		
		if (isAdmin && !context.Message.RoleIds.Contains(RoleEnum.Administrator) && httpContextAccessor.GetUserId() == context.Message.Id)
		{
			await RespondWithValidationFailAsync(context, "RoleIds", "Administrator nie może odebrać sobie uprawnień administratora");
			return false;
		}
		
		if (await users.GetAll().AnyAsync(u => u.Name == context.Message.Username && u.Id != context.Message.Id))
		{
			await RespondWithValidationFailAsync(context, "Username", "Istnieje inny użytkownik o takiej nazwie");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<EditUserOrder> context)
	{	
		var newRoles = await roles.GetAll().Where(r => context.Message.RoleIds.Contains(r.Id)).ToListAsync();
		var user = await users.GetAll()
			.Include(x => x.Roles)
			.FirstOrDefaultAsync(u => u.Id == context.Message.Id) ??
			throw new UPSException("No user");
			
		logger.LogInformation("Editing user {UserId}", user.Id);
		
		if (user.Roles.Any(r => r.Id == RoleEnum.Administrator) && !httpContextAccessor.HasAnyRole(RoleEnum.Administrator))
		{
			await RespondWithValidationFailAsync(context, "Id", "Nie można edytować administratora");
			return;
		}
			
		user.Name = context.Message.Username;
		user.Roles = newRoles;
		if (!string.IsNullOrEmpty(context.Message.Password))
		{
			logger.LogInformation("Changing password for edited user");
			var salt = passwordService.GenerateSalt();
			var hash = passwordService.GenerateHash(context.Message.Password, salt);
			user.Hash = hash;
			user.Salt = salt;
		}
		
		await users.UpdateAsync(user);

		if (user.Id == httpContextAccessor.GetUserId())
			editedUser = user;
	}

	public override async Task PostTransaction(ConsumeContext<EditUserOrder> context)
	{
		if (editedUser != null)
		{
			logger.LogInformation("Updating claims after self edit");
			var claimsIdentity = new ClaimsIdentity(editedUser.GetClaims(), CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(claimsIdentity);
			await httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
		}
		
		await RespondAsync(context, new EditUserResponse());
	}
}