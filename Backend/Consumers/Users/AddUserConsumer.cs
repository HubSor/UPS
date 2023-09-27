using Core;
using Helpers;
using MassTransit;
using Messages.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Services;

namespace Consumers.Users;
public class AddUserConsumer : BaseConsumer<AddUserOrder, AddUserResponse>
{
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IRepository<User> users;
	private readonly IRepository<Role> roles;
	private readonly IPasswordService passwordService;
	
	public AddUserConsumer(IHttpContextAccessor httpContextAccessor, ILogger<AddUserConsumer> logger, IRepository<User> users, IPasswordService passwordService,
		IRepository<Role> roles)
		: base(logger)
	{
		this.roles = roles;
		this.users = users;
		this.passwordService = passwordService;
		this.httpContextAccessor = httpContextAccessor;
	}

	public override async Task Consume(ConsumeContext<AddUserOrder> context)
	{
		if (!httpContextAccessor.HasAnyRole(RoleEnum.UserManager, RoleEnum.Administrator))
		{
			await RespondWithValidationFailAsync(context, "root", "Brak uprawnień do tworzenia użykowników");
			return;
		}
		
		if (await users.GetAll().AnyAsync(x => x.Name == context.Message.Username))
		{
			await RespondWithValidationFailAsync(context, "Username", "Istnieje już użytkownik o takiej nazwie");
			return;
		}
		
		var salt = passwordService.GenerateSalt();
		var hash = passwordService.GenerateHash(context.Message.Password, salt);
		var newRoles = await roles.GetAll().Where(r => context.Message.RoleIds.Contains(r.Id)).ToListAsync();
		
		var newUser = new User()
		{
			Active = true,
			Name = context.Message.Username,
			Hash = hash,
			Salt = salt,
			Roles = newRoles,
		};
		
		await users.AddAsync(newUser);
		await RespondAsync(context, new AddUserResponse());
	}
}
