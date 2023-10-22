using Core;
using Data;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class DeleteSubProductConsumer : TransactionConsumer<DeleteSubProductOrder, DeleteSubProductResponse>
{
	private readonly IRepository<SubProduct> subProducts;
	private readonly IRepository<SubProductInProduct> subProductsInProducts;
	private readonly IRepository<Parameter> parameters;
	
	public DeleteSubProductConsumer(ILogger<DeleteSubProductConsumer> logger, IRepository<SubProduct> subProducts,
		IRepository<SubProductInProduct> subProductsInProducts, IRepository<Parameter> parameters,
		IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.subProducts = subProducts;
		this.subProductsInProducts = subProductsInProducts;
		this.parameters = parameters;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<DeleteSubProductOrder> context)
	{		
		if (!await subProducts.GetAll().AnyAsync(x => x.Id == context.Message.SubProductId))
		{
			await RespondWithValidationFailAsync(context, "SubProductId", "Nie znaleziono podproduktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<DeleteSubProductOrder> context)
	{
		var subProduct = await subProducts.GetAll()
			.Include(p => p.SubProductInProducts)
			.Include(p => p.SubProductInSales)
			.Include(p => p.Parameters)
			.ThenInclude(p => p.SaleParameters)
			.FirstAsync(p => p.Id == context.Message.SubProductId);

		foreach (var assignment in subProduct.SubProductInProducts.ToList())
		{
			await subProductsInProducts.DeleteAsync(assignment);
			subProduct.SubProductInProducts.Remove(assignment);
		}
			
		foreach(var param in subProduct.Parameters.ToList())
		{
			if (param.SaleParameters.Any())
			{
				param.Deleted = true;
				await parameters.UpdateAsync(param);
			}
			else
			{
				await parameters.DeleteAsync(param);
				subProduct.Parameters.Remove(param);
			}
		}
		
		if (subProduct.SubProductInSales.Any() || subProduct.Parameters.Any(p => p.SaleParameters.Any()))
		{
			subProduct.Deleted = true;
			await subProducts.UpdateAsync(subProduct);
		}
		else 
		{
			await subProducts.DeleteAsync(subProduct);	
		}
		
		await unitOfWork.FlushAsync();
	}

	public override async Task PostTransaction(ConsumeContext<DeleteSubProductOrder> context)
	{
		await RespondAsync(context, new DeleteSubProductResponse());
	}
}
