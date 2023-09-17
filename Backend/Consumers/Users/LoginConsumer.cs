using System.Security.Claims;
using Core;
using MassTransit;
using Messages.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Services;

namespace Consumers.Products;
public class LoginConsumer : TransactionConsumer<LoginOrder, LoginResponse>
{
	private IRepository<User> users;
	private IHttpContextAccessor httpContextAccessor;
	private IPasswordService passwordService;
	public LoginConsumer(IUnitOfWork unitOfWork, IRepository<User> users, IHttpContextAccessor httpContextAccessor, ILogger<LoginConsumer> logger, IPasswordService passwordService)
		: base(unitOfWork, logger)
	{
		this.users = users;
		this.httpContextAccessor = httpContextAccessor;
		this.passwordService = passwordService;
	}

	public override async Task InTransaction(ConsumeContext<LoginOrder> context)
	{
		var user = await users.GetAll().FirstOrDefaultAsync(u => u.Name == context.Message.Username);
		
		if (user == null || !user.Active)
		{
			passwordService.FakeGenerateHash();
			await RespondWithValidationFailAsync(context, nameof(LoginOrder.Password), "Niepoprawne hasło");
			return;
		}
		
		if (passwordService.GenerateHash(context.Message.Password, user.Salt) == user.Hash)
		{
			var claims = user.GetClaims();
			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(claimsIdentity);
			await httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
			await RespondAsync(context, new LoginResponse());			
			return;
		}
		
		if (!user.Active)
		{
			await RespondWithValidationFailAsync(context, nameof(LoginOrder.Username), "Konto nieaktywne");	
		}
		
		await RespondWithValidationFailAsync(context, nameof(LoginOrder.Password), "Niepoprawne hasło");
	}
}
