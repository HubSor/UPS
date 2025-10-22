using Core;
using Core.Data;
using Core.Messages;
using Core.Models;
using Core.Web;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace UsersMicro.Consumers;

public class DeleteUserConsumer : TransactionConsumer<DeleteUserOrder, DeleteUserResponse>
{
	private readonly IRepository<User> users;
	private User? user;
	
	public DeleteUserConsumer(ILogger<DeleteUserConsumer> logger, IRepository<User> users, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.users = users;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<DeleteUserOrder> context)
	{
		user = await users.GetAll()
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(x => x.Id == context.Message.Id);
			
		if (user == null)
		{
			await RespondWithValidationFailAsync(context, "Id", "Nie znaleziono użytkownika");
			return false;
		}
		
		if (user.Id == context.Message.GetUserId())
		{
			await RespondWithValidationFailAsync(context, "Id", "Nie można usunąć samego siebie");
			return false;
		}
		
		if (user.Roles.Any(r => r.Id == RoleEnum.Administrator) && !context.Message.HasAnyRole(RoleEnum.Administrator))
		{
			await RespondWithValidationFailAsync(context, "Id", "Nie można usunąć administratora");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<DeleteUserOrder> context)
	{
		if (user != null)
			await users.DeleteAsync(user);

		logger.LogInformation("Deleted user {UserId}", user?.Id);
	}

	public override async Task PostTransaction(ConsumeContext<DeleteUserOrder> context)
	{
		await RespondAsync(context, new DeleteUserResponse());
	}
}
