using Core;
using Data;
using MassTransit;
using Messages.Parameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Parameters;
public class AddParameterConsumer : TransactionConsumer<AddParameterOrder, AddParameterResponse>
{
	private readonly IRepository<Parameter> parameters;
	private readonly IRepository<SubProduct> subProducts;
	private readonly IRepository<Product> products;
	
	public AddParameterConsumer(ILogger<AddParameterConsumer> logger, IRepository<Parameter> parameters,
		IRepository<Product> products, IRepository<SubProduct> subProducts, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.parameters = parameters;
		this.products = products;
		this.subProducts = subProducts;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<AddParameterOrder> context)
	{
		if (context.Message.ProductId.HasValue && !await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductId))
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return false;
		}

		if (context.Message.SubProductId.HasValue && !await subProducts.GetAll().AnyAsync(x => x.Id == context.Message.SubProductId))
		{
			await RespondWithValidationFailAsync(context, "SubProductId", "Nie znaleziono podproduktu");
			return false;
		}

		return true;
	}

	public override async Task InTransaction(ConsumeContext<AddParameterOrder> context)
	{
		var parameter = new Parameter()
		{
			Name = context.Message.Name,
			Required = context.Message.Required,
			Type = context.Message.Type,
			ProductId = context.Message.ProductId,
			SubProductId = context.Message.SubProductId
		};
		
		parameter.Options = context.Message.Options?.Select(x => new ParameterOption()
		{
			Value = x.Value,
			Parameter = parameter
		}).ToList() ?? new List<ParameterOption>();
		
		await parameters.AddAsync(parameter);
	}

	public override async Task PostTransaction(ConsumeContext<AddParameterOrder> context)
	{
		await RespondAsync(context, new AddParameterResponse());
	}
}
