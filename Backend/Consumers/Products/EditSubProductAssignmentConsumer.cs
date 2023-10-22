using Core;
using Data;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class EditSubProductAssignmentConsumer : TransactionConsumer<EditSubProductAssignmentOrder, EditSubProductAssignmentResponse>
{
	private readonly IRepository<SubProductInProduct> subProductsInProducts;
	private SubProductInProduct? editedEntity;
	
	public EditSubProductAssignmentConsumer(ILogger<EditSubProductAssignmentConsumer> logger, IRepository<SubProductInProduct> subProductsInProducts, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.subProductsInProducts = subProductsInProducts;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<EditSubProductAssignmentOrder> context)
	{	
		editedEntity = await subProductsInProducts.GetAll()
			.FirstOrDefaultAsync(x => x.SubProductId == context.Message.SubProductId && x.ProductId == context.Message.ProductId);
		if (editedEntity == null)
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono przypisania tego podproduktu do tego produktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<EditSubProductAssignmentOrder> context)
	{
		if (editedEntity == null)
			throw new UPSException("No SubProductInProduct");
			
		editedEntity.InProductPrice = context.Message.NewPrice;
		
		await subProductsInProducts.UpdateAsync(editedEntity);
	}

	public override async Task PostTransaction(ConsumeContext<EditSubProductAssignmentOrder> context)
	{
		await RespondAsync(context, new EditSubProductAssignmentResponse());
	}
}
