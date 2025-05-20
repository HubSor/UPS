using System.Security.Claims;
using Core;
using Dtos.Users;
using MassTransit;
using Messages.Queries;
using Messages.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Services.Domain;

namespace Consumers.Query;
public class LoginConsumer : BaseQueryConsumer<LoginQuery, LoginResponse>
{
	private readonly IRepository<User> users;
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IPasswordService passwordService;
	
	public LoginConsumer(IRepository<User> users, IHttpContextAccessor httpContextAccessor, ILogger<LoginConsumer> logger, IPasswordService passwordService)
		: base(logger)
	{
		this.users = users;
		this.httpContextAccessor = httpContextAccessor;
		this.passwordService = passwordService;
	}

	public override async Task Consume(ConsumeContext<LoginQuery> context)
	{
		logger.LogInformation("Login initiated");
		var user = await users.GetAll().Include(x => x.Roles).FirstOrDefaultAsync(u => u.Name == context.Message.Username);
		
		if (user == null || !user.Active)
		{
			logger.LogInformation("Faking login");
			passwordService.FakeGenerateHash();
			await RespondWithValidationFailAsync(context, nameof(LoginQuery.Password), "Niepoprawne hasło");
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
			await RespondWithValidationFailAsync(context, nameof(LoginQuery.Username), "Konto nieaktywne");
			return;
		}
		
		await RespondWithValidationFailAsync(context, nameof(LoginQuery.Password), "Niepoprawne hasło");
	}
}
