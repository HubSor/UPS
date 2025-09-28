using Core;
using Core.Data;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicro.Consumers;

public class EditSubProductConsumer : TransactionConsumer<EditSubProductOrder, EditSubProductResponse>
{
	private readonly IRepository<SubProduct> subProducts;
	
	public EditSubProductConsumer(ILogger<EditSubProductConsumer> logger, IRepository<SubProduct> subProducts, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.subProducts = subProducts;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<EditSubProductOrder> context)
	{
		if (await subProducts.GetAll().AnyAsync(x => x.Code == context.Message.Code.ToUpper() && x.Id != context.Message.Id))
		{
			await RespondWithValidationFailAsync(context, "Code", "Istnieje już inny podprodukt o takim kodzie");
			return false;
		}
		
		if (!await subProducts.GetAll().AnyAsync(x => x.Id == context.Message.Id && !x.Deleted))
		{
			await RespondWithValidationFailAsync(context, "Id", "Nie znaleziono podproduktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<EditSubProductOrder> context)
	{
		var subProduct = await subProducts.GetAll().FirstAsync(p => p.Id == context.Message.Id);
		
		subProduct.Description = context.Message.Description;
		subProduct.Name = context.Message.Name;
		subProduct.Code = context.Message.Code;
		subProduct.BasePrice = context.Message.BasePrice;
		subProduct.TaxRate = context.Message.TaxRate != 0 ? context.Message.TaxRate / 100m : 0.00m;

		await subProducts.UpdateAsync(subProduct);
		logger.LogInformation("Edited subproduct {SubProductId}", subProduct.Id);
	}

	public override async Task PostTransaction(ConsumeContext<EditSubProductOrder> context)
	{
		await RespondAsync(context, new EditSubProductResponse());
	}
}
