using Core;
using Dtos.Products;
using MassTransit;
using Messages.Queries;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.QueryConsumers;
public class GetProductConsumer : BaseQueryConsumer<GetProductQuery, GetProductResponse>
{
	private readonly IRepository<Product> products;

	public GetProductConsumer(ILogger<GetProductConsumer> logger, IRepository<Product> products)
	: base(logger)
	{
		this.products = products;
	}

	public override async Task Consume(ConsumeContext<GetProductQuery> context)
	{
		if (!await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductId && !x.Deleted))
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return;
		}

		var product = await products.GetAll()
			.Include(p => p.Parameters)
			.ThenInclude(o => o.Options)
			.Include(p => p.SubProductInProducts)
			.ThenInclude(sp => sp.SubProduct)
			.ThenInclude(sp => sp.Parameters)
			.ThenInclude(p => p.Options)
			.FirstAsync(p => p.Id == context.Message.ProductId);

		var productDto = new ExtendedProductDto(product);
		logger.LogInformation("Got product {ProductId}", product.Id);

		await RespondAsync(context, new GetProductResponse()
		{
			Product = productDto
		});
	}
}
