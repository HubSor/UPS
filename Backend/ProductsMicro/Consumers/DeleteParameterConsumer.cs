using Core;
using Core.Data;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicro.Consumers;

public class DeleteParameterConsumer : TransactionConsumer<DeleteParameterOrder, DeleteParameterResponse>
{
	private readonly IRepository<Parameter> parameters;
	private Parameter parameter = default!;
	
	public DeleteParameterConsumer(ILogger<DeleteParameterConsumer> logger, IRepository<Parameter> parameters, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.parameters = parameters;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<DeleteParameterOrder> context)
	{
		var param = await parameters.GetAll()
			.Include(x => x.SaleParameters)
			.FirstOrDefaultAsync(x => x.Id == context.Message.ParameterId && !x.Deleted);
			
		if (param == null)
		{
			await RespondWithValidationFailAsync(context, "ParameterId", "Nie znaleziono parametru");
			return false;
		}
		
		parameter = param;
		return true;
	}

	public override async Task InTransaction(ConsumeContext<DeleteParameterOrder> context)
	{
		if (parameter.SaleParameters.Any())
		{
			parameter.Deleted = true;
			await parameters.UpdateAsync(parameter);
			logger.LogInformation("Soft deleted parameter {ParameterId}", parameter.Id);
		}
		else 
		{
			await parameters.DeleteAsync(parameter);
			logger.LogInformation("Hard deleted parameter {ParameterId}", parameter.Id);
		}
	}

	public override async Task PostTransaction(ConsumeContext<DeleteParameterOrder> context)
	{
		await RespondAsync(context, new DeleteParameterResponse());
	}
}
