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
		if (!await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductId))
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<GetProductOrder> context)
	{
		var product = await products.GetAll()
			.Include(p => p.SubProductInProducts)
			.ThenInclude(sp => sp.SubProduct)
			.FirstAsync(p => p.Id == context.Message.ProductId);

		productDto = new ExtendedProductDto(product);
	}

	public override async Task PostTransaction(ConsumeContext<GetProductOrder> context)
	{
		await RespondAsync(context, new GetProductResponse() 
		{
			Product = productDto
		});
	}
}
