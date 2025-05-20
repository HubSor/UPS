using Core;
using Dtos.Products;
using MassTransit;
using Messages.Queries;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.QueryConsumers;
public class GetSubProductConsumer : BaseQueryConsumer<GetSubProductQuery, GetSubProductResponse>
{
	private readonly IRepository<SubProduct> subProducts;

	public GetSubProductConsumer(ILogger<GetSubProductConsumer> logger, IRepository<SubProduct> subProducts)
	: base(logger)
	{
		this.subProducts = subProducts;
	}

	public override async Task Consume(ConsumeContext<GetSubProductQuery> context)
	{
		if (!await subProducts.GetAll().AnyAsync(x => x.Id == context.Message.SubProductId && !x.Deleted))
		{
			await RespondWithValidationFailAsync(context, "SubProductId", "Nie znaleziono podproduktu");
			return;
		}

		var subProduct = await subProducts.GetAll()
			.Include(p => p.Parameters)
			.ThenInclude(o => o.Options)
			.Include(p => p.SubProductInProducts)
			.ThenInclude(sp => sp.Product)
			.FirstAsync(p => p.Id == context.Message.SubProductId);

		var subProductDto = new ExtendedSubProductDto(subProduct);
		logger.LogInformation("Got subproduct {SubProductId}", subProduct.Id);

		await RespondAsync(context, new GetSubProductResponse()
		{
			SubProduct = subProductDto
		});
	}
}
