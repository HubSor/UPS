using Core;
using Data;
using MassTransit;
using Messages.Parameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Parameters;
public class DeleteOptionConsumer : TransactionConsumer<DeleteOptionOrder, DeleteOptionResponse>
{
	private readonly IRepository<ParameterOption> options;
	private ParameterOption option = default!;
	
	public DeleteOptionConsumer(ILogger<DeleteOptionConsumer> logger, IRepository<ParameterOption> options, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.options = options;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<DeleteOptionOrder> context)
	{
		var opt = await options.GetAll()
			.FirstOrDefaultAsync(x => x.Id == context.Message.OptionId);
			
		if (opt == null)
		{
			await RespondWithValidationFailAsync(context, "OptionId", "Nie znaleziono opcji");
			return false;
		}
		
		option = opt;
		return true;
	}

	public override async Task InTransaction(ConsumeContext<DeleteOptionOrder> context)
	{
		await options.DeleteAsync(option);	
	}

	public override async Task PostTransaction(ConsumeContext<DeleteOptionOrder> context)
	{
		await RespondAsync(context, new DeleteOptionResponse());
	}
}
