using Core;
using Data;
using Dtos.Products;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class GetProductConsumer : TransactionConsumer<GetProductOrder, GetProductResponse>
{
	private readonly IRepository<Product> products;
	private ExtendedProductDto productDto = default!;

	public GetProductConsumer(ILogger<GetProductConsumer> logger, IRepository<Product> products, IUnitOfWork unitOfWork)
	: base(unitOfWork, logger)
	{
		this.products = products;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<GetProductOrder> context)
	{	
		if (!await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductId && !x.Deleted))
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<GetProductOrder> context)
	{
		var product = await products.GetAll()
			.Include(p => p.Parameters)
			.ThenInclude(o => o.Options)
			.Include(p => p.SubProductInProducts)
			.ThenInclude(sp => sp.SubProduct)
			.ThenInclude(sp => sp.Parameters)
			.ThenInclude(p => p.Options)
			.FirstAsync(p => p.Id == context.Message.ProductId);

		productDto = new ExtendedProductDto(product);
		logger.LogInformation("Got product {ProductId}", product.Id);
	}

	public override async Task PostTransaction(ConsumeContext<GetProductOrder> context)
	{
		await RespondAsync(context, new GetProductResponse() 
		{
			Product = productDto
		});
	}
}
