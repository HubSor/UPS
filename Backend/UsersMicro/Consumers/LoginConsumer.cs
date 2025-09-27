using System.Security.Claims;
using Core;
using Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UsersMicro.Dtos;
using UsersMicro.Messages;
using UsersMicro.Models;
using UsersMicro.Services;
using WebCommons;

namespace UsersMicro.Consumers;

public class LoginConsumer : TransactionConsumer<LoginOrder, LoginResponse>
{
	private readonly IRepository<User> users;
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IPasswordService passwordService;
	public LoginConsumer(IUnitOfWork unitOfWork, IRepository<User> users, IHttpContextAccessor httpContextAccessor, ILogger<LoginConsumer> logger, IPasswordService passwordService)
		: base(unitOfWork, logger)
	{
		this.users = users;
		this.httpContextAccessor = httpContextAccessor;
		this.passwordService = passwordService;
	}

	public override async Task InTransaction(ConsumeContext<LoginOrder> context)
	{
		logger.LogInformation("Login initiated");
		var user = await users.GetAll().Include(x => x.Roles).FirstOrDefaultAsync(u => u.Name == context.Message.Username);
		
		if (user == null || !user.Active)
		{
			logger.LogInformation("Faking login");
			passwordService.FakeGenerateHash();
			await RespondWithValidationFailAsync(context, nameof(LoginOrder.Password), "Niepoprawne hasło");
			return;
		}
		
		var newHash = passwordService.GenerateHash(context.Message.Password, user.Salt);
		if (newHash.SequenceEqual(user.Hash))
		{
			var claims = user.GetClaims();
			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(claimsIdentity);
			await httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
			await RespondAsync(context, new LoginResponse() 
			{
				UserDto = new UserDto()
				{
					Id = user.Id,
					Username = user.Name,
					Roles = user.Roles.Select(r => r.Id.ToString()).ToList()
				}
			});			
			return;
		}
		
		if (!user.Active)
		{
			await RespondWithValidationFailAsync(context, nameof(LoginOrder.Username), "Konto nieaktywne");
			return;
		}
		
		await RespondWithValidationFailAsync(context, nameof(LoginOrder.Password), "Niepoprawne hasło");
	}
}
