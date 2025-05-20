using Core;
using Data;
using MassTransit;
using Messages.Commands;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Command;
public class UnassignSubProductsConsumer : BaseCommandConsumer<UnassignSubProductsOrder, UnassignSubProductsResponse>
{
	private readonly IRepository<SubProduct> subProducts;
	private readonly IRepository<Product> products;
	private readonly IRepository<SubProductInProduct> subProductsInProducts;
	
	public UnassignSubProductsConsumer(ILogger<UnassignSubProductsConsumer> logger, IRepository<SubProduct> subProducts,
		IRepository<Product> products, IRepository<SubProductInProduct> subProductsInProducts, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.subProducts = subProducts;
		this.products = products;
		this.subProductsInProducts = subProductsInProducts;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<UnassignSubProductsOrder> context)
	{
		var subProductIds = await subProducts.GetAll().Select(x => x.Id).ToListAsync();
		if (!context.Message.SubProductIds.All(x => subProductIds.Contains(x)))
		{
			await RespondWithValidationFailAsync(context, "SubProductIds", "Nie znaleziono podproduktu");
			return false;
		}
		
		if (!await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductId))
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return false;
		}
		
		if (!await subProductsInProducts.GetAll()
			.AnyAsync(x => context.Message.SubProductIds.Contains(x.SubProductId) && context.Message.ProductId == x.ProductId))
		{
			await RespondAsync(context, new UnassignSubProductsResponse());
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<UnassignSubProductsOrder> context)
	{
		var toDeleteList = await subProductsInProducts.GetAll()
			.Where(x => context.Message.SubProductIds.Contains(x.SubProductId) && context.Message.ProductId == x.ProductId).ToListAsync();
		foreach (var toDelete in toDeleteList)
		{
			logger.LogInformation("Deleting assignment of subproduct {SubProductId} to product {ProductId}", toDelete.SubProductId, toDelete.ProductId);
			await subProductsInProducts.DeleteAsync(toDelete);	
		}
	}

	public override async Task PostTransaction(ConsumeContext<UnassignSubProductsOrder> context)
	{
		await RespondAsync(context, new UnassignSubProductsResponse());
	}
}
