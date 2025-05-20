using Core;
using Data;
using MassTransit;
using Messages.Orders;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.CommandConsumers;
public class EditSubProductAssignmentConsumer : BaseCommandConsumer<EditSubProductAssignmentOrder, EditSubProductAssignmentResponse>
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
		logger.LogInformation("Edited subproduct assignment for product {ProductId} and subproduct {SubProductId}",
			editedEntity.ProductId, editedEntity.SubProductId);
	}

	public override async Task PostTransaction(ConsumeContext<EditSubProductAssignmentOrder> context)
	{
		await RespondAsync(context, new EditSubProductAssignmentResponse());
	}
}
