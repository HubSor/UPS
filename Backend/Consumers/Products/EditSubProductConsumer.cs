using Core;
using Data;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
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
		if (await subProducts.GetAll().AnyAsync(x => x.Code == context.Message.SubProductDto.Code.ToUpper() && x.Id != context.Message.SubProductDto.Id))
		{
			await RespondWithValidationFailAsync(context, "Code", "Istnieje już inny podprodukt o takim kodzie");
			return false;
		}
		
		if (!await subProducts.GetAll().AnyAsync(x => x.Id == context.Message.SubProductDto.Id))
		{
			await RespondWithValidationFailAsync(context, "Id", "Nie znaleziono podproduktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<EditSubProductOrder> context)
	{
		var subProduct = await subProducts.GetAll().FirstAsync(p => p.Id == context.Message.SubProductDto.Id);
		
		subProduct.Description = context.Message.SubProductDto.Description;
		subProduct.Name = context.Message.SubProductDto.Name;
		subProduct.Code = context.Message.SubProductDto.Code;
		subProduct.BasePrice = context.Message.SubProductDto.BasePrice;
		
		await subProducts.UpdateAsync(subProduct);
	}

	public override async Task PostTransaction(ConsumeContext<EditSubProductOrder> context)
	{
		await RespondAsync(context, new EditSubProductResponse());
	}
}
