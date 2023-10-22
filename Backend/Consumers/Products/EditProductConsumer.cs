using Core;
using Data;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class EditProductConsumer : TransactionConsumer<EditProductOrder, EditProductResponse>
{
	private readonly IRepository<Product> products;
	
	public EditProductConsumer(ILogger<EditProductConsumer> logger, IRepository<Product> products, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.products = products;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<EditProductOrder> context)
	{
		if (await products.GetAll().AnyAsync(x => x.Code == context.Message.ProductDto.Code.ToUpper() && x.Id != context.Message.ProductDto.Id))
		{
			await RespondWithValidationFailAsync(context, "Code", "Istnieje już inny produkt o takim kodzie");
			return false;
		}
		
		if (!await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductDto.Id && !x.Deleted))
		{
			await RespondWithValidationFailAsync(context, "Id", "Nie znaleziono produktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<EditProductOrder> context)
	{
		var product = await products.GetAll().FirstAsync(p => p.Id == context.Message.ProductDto.Id);
		
		product.Description = context.Message.ProductDto.Description;
		product.Name = context.Message.ProductDto.Name;
		product.Code = context.Message.ProductDto.Code;
		product.BasePrice = context.Message.ProductDto.BasePrice;
		product.AnonymousSaleAllowed = context.Message.ProductDto.AnonymousSaleAllowed;
		product.Status = context.Message.ProductDto.Status;
		
		await products.UpdateAsync(product);
	}

	public override async Task PostTransaction(ConsumeContext<EditProductOrder> context)
	{
		await RespondAsync(context, new EditProductResponse());
	}
}
