using Core;
using Data;
using MassTransit;
using Messages.Orders;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Parameters;
public class EditParameterConsumer : BaseCommandConsumer<EditParameterOrder, EditParameterResponse>
{
	private readonly IRepository<Parameter> parameters;
	private readonly IRepository<ParameterOption> options;
	private Parameter parameter = default!;
	
	public EditParameterConsumer(ILogger<EditParameterConsumer> logger, IRepository<Parameter> parameters, IRepository<ParameterOption> options, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.parameters = parameters;
		this.options = options;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<EditParameterOrder> context)
	{
		var param = await parameters.GetAll()
			.Include(x => x.Options)
			.FirstOrDefaultAsync(x => x.Id == context.Message.ParameterId && !x.Deleted);
			
		if (param == null)
		{
			await RespondWithValidationFailAsync(context, "ParameterId", "Nie znaleziono parametru");
			return false;
		}
		
		parameter = param;
		return true;
	}

	public override async Task InTransaction(ConsumeContext<EditParameterOrder> context)
	{
		logger.LogInformation("Editing parameter {ParameterId}", parameter.Id);
		parameter.Required = context.Message.Required;
		parameter.Name = context.Message.Name;
		
		var oldAllowsOptions = parameter.Type.AllowsOptions();
		var newAllowsOptions = context.Message.Type.AllowsOptions();
		parameter.Type = context.Message.Type;

		if (oldAllowsOptions)
		{
			logger.LogInformation("Deleting options of old parameter");
			foreach (var opt in parameter.Options.ToList())
			{
				await options.DeleteAsync(opt);
				parameter.Options.Remove(opt);
			}
		}
		
		if (newAllowsOptions)
		{
			logger.LogInformation("Adding options for new parameter");
			parameter.Options = context.Message.Options?.Select(x => new ParameterOption()
			{
				Value = x.Value,
				Parameter = parameter
			}).ToList() ?? new List<ParameterOption>();
		}

		await parameters.UpdateAsync(parameter);
		logger.LogInformation("Edited parameter {ParameterId}", parameter.Id);
	}

	public override async Task PostTransaction(ConsumeContext<EditParameterOrder> context)
	{
		await RespondAsync(context, new EditParameterResponse());
	}
}
