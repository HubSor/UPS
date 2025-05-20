using Core;
using Data;
using MassTransit;
using Messages.Orders;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class AssignSubProductConsumer : BaseCommandConsumer<AssignSubProductOrder, AssignSubProductResponse>
{
	private readonly IRepository<SubProduct> subProducts;
	private readonly IRepository<Product> products;
	private readonly IRepository<SubProductInProduct> subProductsInProducts;
	
	public AssignSubProductConsumer(ILogger<AssignSubProductConsumer> logger, IRepository<SubProduct> subProducts,
		IRepository<Product> products, IRepository<SubProductInProduct> subProductsInProducts, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.subProducts = subProducts;
		this.products = products;
		this.subProductsInProducts = subProductsInProducts;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<AssignSubProductOrder> context)
	{
		if (!await subProducts.GetAll().AnyAsync(x => x.Id == context.Message.SubProductId && !x.Deleted))
		{
			await RespondWithValidationFailAsync(context, "SubProductId", "Nie znaleziono podproduktu");
			return false;
		}
		
		if (!await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductId && !x.Deleted))
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return false;
		}
		
		if (await subProductsInProducts.GetAll().AnyAsync(x => x.SubProductId == context.Message.SubProductId && x.ProductId == context.Message.ProductId))
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Podprodukt jest już przypisany do tego produktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<AssignSubProductOrder> context)
	{
		var subProductInProduct = new SubProductInProduct()
		{
			SubProductId = context.Message.SubProductId,
			ProductId = context.Message.ProductId,
			InProductPrice = context.Message.Price,
		};
		
		await subProductsInProducts.AddAsync(subProductInProduct);
		logger.LogInformation("Assigned subproduct {SubProductId} to product {ProductId}", subProductInProduct.SubProductId, context.Message.ProductId);
	}

	public override async Task PostTransaction(ConsumeContext<AssignSubProductOrder> context)
	{
		await RespondAsync(context, new AssignSubProductResponse());
	}
}
