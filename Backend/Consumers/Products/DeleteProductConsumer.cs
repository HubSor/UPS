using Core;
using Data;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class DeleteProductConsumer : TransactionConsumer<DeleteProductOrder, DeleteProductResponse>
{
	private readonly IRepository<Product> products;
	private readonly IRepository<SubProductInProduct> subProductsInProducts;
	private readonly IRepository<Parameter> parameters;

	public DeleteProductConsumer(ILogger<DeleteProductConsumer> logger, IRepository<Product> products,
		IRepository<SubProductInProduct> subProductsInProducts, IRepository<Parameter> parameters, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.products = products;
		this.subProductsInProducts = subProductsInProducts;
		this.parameters = parameters;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<DeleteProductOrder> context)
	{	
		if (!await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductId))
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<DeleteProductOrder> context)
	{
		var product = await products.GetAll()
			.Include(p => p.SubProductInProducts)
			.Include(p => p.Sales)
			.Include(p => p.Parameters)
			.ThenInclude(p => p.SaleParameters)
			.FirstAsync(p => p.Id == context.Message.ProductId);


		foreach (var assignment in product.SubProductInProducts.ToList())
		{
			await subProductsInProducts.DeleteAsync(assignment);
			product.SubProductInProducts.Remove(assignment);
		}

		foreach (var param in product.Parameters.ToList())
		{
			if (param.SaleParameters.Any())
			{
				param.Deleted = true;
				await parameters.UpdateAsync(param);
			}
			else
			{
				await parameters.DeleteAsync(param);
				product.Parameters.Remove(param);
			}
		}

		if (product.Sales.Any() || product.Parameters.Any(p => p.SaleParameters.Any()))
		{
			product.Deleted = true;
			await products.UpdateAsync(product);
		}
		else
		{
			await products.DeleteAsync(product);
		}

		await unitOfWork.FlushAsync();
	}

	public override async Task PostTransaction(ConsumeContext<DeleteProductOrder> context)
	{
		await RespondAsync(context, new DeleteProductResponse());
	}
}
