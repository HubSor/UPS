using Core;
using Data;
using MassTransit;
using Messages.Orders;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Parameters;
public class AddOptionConsumer : BaseCommandConsumer<AddOptionOrder, AddOptionResponse>
{
	private readonly IRepository<Parameter> parameters;
	private readonly IRepository<ParameterOption> options;
	private Parameter parameter = default!;
	
	public AddOptionConsumer(ILogger<AddOptionConsumer> logger, IRepository<Parameter> parameters,
		IRepository<ParameterOption> options, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.parameters = parameters;
		this.options = options;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<AddOptionOrder> context)
	{
		var param = await parameters.GetAll().FirstOrDefaultAsync(x => x.Id == context.Message.ParameterId && !x.Deleted);
		if (param == null)
		{
			await RespondWithValidationFailAsync(context, "ParameterId", "Nie znaleziono parametru");
			return false;
		}

		if (!param.Type.AllowsOptions())
		{
			await RespondWithValidationFailAsync(context, "ParameterId", "Typ parametru nie pozwala na dodanie opcji");
			return false;
		}
		
		parameter = param;
		return true;
	}

	public override async Task InTransaction(ConsumeContext<AddOptionOrder> context)
	{
		var option = new ParameterOption()
		{
			Value = context.Message.Value,
			ParameterId = parameter.Id
		};
		await options.AddAsync(option);
		logger.LogInformation("Added option to parameter {ParameterId}", parameter.Id);
	}

	public override async Task PostTransaction(ConsumeContext<AddOptionOrder> context)
	{
		await RespondAsync(context, new AddOptionResponse());
	}
}
