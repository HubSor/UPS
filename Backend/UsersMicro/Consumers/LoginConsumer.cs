using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UsersMicro.Services;

namespace UsersMicro.Consumers;

public class LoginConsumer : TransactionConsumer<LoginOrder, LoginResponse>
{
	private readonly IRepository<User> users;
	private readonly IPasswordService passwordService;
	public LoginConsumer(IUnitOfWork unitOfWork, IRepository<User> users, ILogger<LoginConsumer> logger, IPasswordService passwordService)
		: base(unitOfWork, logger)
	{
		this.users = users;
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
			await RespondAsync(context, new LoginResponse()
			{
				UserDto = new UserDto()
				{
					Id = user.Id,
					Username = user.Name,
					Roles = user.Roles.Select(r => r.Id.ToString()).ToList()
				},
				Claims = user.GetClaims().Select(x => new ClaimDto(x)).ToList(),
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
