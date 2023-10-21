using Core;
using Data;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class AssignSubProductToProductConsumer : TransactionConsumer<AssignSubProductToProductOrder, AssignSubProductToProductResponse>
{
	private readonly IRepository<SubProduct> subProducts;
	private readonly IRepository<Product> products;
	private readonly IRepository<SubProductInProduct> subProductsInProducts;
	
	public AssignSubProductToProductConsumer(ILogger<AssignSubProductToProductConsumer> logger, IRepository<SubProduct> subProducts,
		IRepository<Product> products, IRepository<SubProductInProduct> subProductsInProducts, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.subProducts = subProducts;
		this.products = products;
		this.subProductsInProducts = subProductsInProducts;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<AssignSubProductToProductOrder> context)
	{
		if (!await subProducts.GetAll().AnyAsync(x => x.Id == context.Message.SubProductId))
		{
			await RespondWithValidationFailAsync(context, "SubProductId", "Nie znaleziono podproduktu");
			return false;
		}
		
		if (!await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductId))
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

	public override async Task InTransaction(ConsumeContext<AssignSubProductToProductOrder> context)
	{
		var subProductInProduct = new SubProductInProduct()
		{
			SubProductId = context.Message.SubProductId,
			ProductId = context.Message.ProductId,
			InProductPrice = context.Message.Price,
		};
		
		await subProductsInProducts.AddAsync(subProductInProduct);
	}

	public override async Task PostTransaction(ConsumeContext<AssignSubProductToProductOrder> context)
	{
		await RespondAsync(context, new AssignSubProductToProductResponse());
	}
}
