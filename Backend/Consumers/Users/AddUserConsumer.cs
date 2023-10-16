using Core;
using Data;
using MassTransit;
using Messages.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Services;

namespace Consumers.Users;
public class AddUserConsumer : TransactionConsumer<AddUserOrder, AddUserResponse>
{
	private readonly IRepository<User> users;
	private readonly IRepository<Role> roles;
	private readonly IPasswordService passwordService;
	
	public AddUserConsumer(ILogger<AddUserConsumer> logger, IRepository<User> users, IPasswordService passwordService,
		IRepository<Role> roles, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.roles = roles;
		this.users = users;
		this.passwordService = passwordService;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<AddUserOrder> context)
	{
		if (await users.GetAll().AnyAsync(x => x.Name == context.Message.Username))
		{
			await RespondWithValidationFailAsync(context, "Username", "Istnieje już użytkownik o takiej nazwie");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<AddUserOrder> context)
	{
		var salt = passwordService.GenerateSalt();
		var hash = passwordService.GenerateHash(context.Message.Password, salt);
		var roleids = roles.GetAll();
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
	}

	public override async Task PostTransaction(ConsumeContext<AddUserOrder> context)
	{
		await RespondAsync(context, new AddUserResponse());
	}
}
