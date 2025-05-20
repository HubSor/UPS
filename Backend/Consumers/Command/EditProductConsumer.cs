using Core;
using Data;
using MassTransit;
using Messages.Commands;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Command;
public class EditProductConsumer : BaseCommandConsumer<EditProductOrder, EditProductResponse>
{
	private readonly IRepository<Product> products;
	
	public EditProductConsumer(ILogger<EditProductConsumer> logger, IRepository<Product> products, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.products = products;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<EditProductOrder> context)
	{
		if (await products.GetAll().AnyAsync(x => x.Code == context.Message.Code.ToUpper() && x.Id != context.Message.Id))
		{
			await RespondWithValidationFailAsync(context, "Code", "Istnieje już inny produkt o takim kodzie");
			return false;
		}
		
		if (!await products.GetAll().AnyAsync(x => x.Id == context.Message.Id && !x.Deleted))
		{
			await RespondWithValidationFailAsync(context, "Id", "Nie znaleziono produktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<EditProductOrder> context)
	{
		var product = await products.GetAll().FirstAsync(p => p.Id == context.Message.Id);
		
		product.Description = context.Message.Description;
		product.Name = context.Message.Name;
		product.Code = context.Message.Code;
		product.BasePrice = context.Message.BasePrice;
		product.AnonymousSaleAllowed = context.Message.AnonymousSaleAllowed;
		product.Status = context.Message.Status;
		product.TaxRate = context.Message.TaxRate != 0 ? context.Message.TaxRate / 100m : 0.00m;

		await products.UpdateAsync(product);
		logger.LogInformation("Edited product {ProductId}", product.Id);
	}

	public override async Task PostTransaction(ConsumeContext<EditProductOrder> context)
	{
		await RespondAsync(context, new EditProductResponse());
	}
}
