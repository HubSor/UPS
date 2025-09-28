using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicro.Consumers;

public class GetSubProductConsumer : TransactionConsumer<GetSubProductOrder, GetSubProductResponse>
{
	private readonly IRepository<SubProduct> subProducts;
	private ExtendedSubProductDto subProductDto = default!;

	public GetSubProductConsumer(ILogger<GetSubProductConsumer> logger, IRepository<SubProduct> subProducts, IUnitOfWork unitOfWork)
	: base(unitOfWork, logger)
	{
		this.subProducts = subProducts;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<GetSubProductOrder> context)
	{	
		if (!await subProducts.GetAll().AnyAsync(x => x.Id == context.Message.SubProductId && !x.Deleted))
		{
			await RespondWithValidationFailAsync(context, "SubProductId", "Nie znaleziono podproduktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<GetSubProductOrder> context)
	{
		var subProduct = await subProducts.GetAll()
			.Include(p => p.Parameters)
			.ThenInclude(o => o.Options)
			.Include(p => p.SubProductInProducts)
			.ThenInclude(sp => sp.Product)
			.FirstAsync(p => p.Id == context.Message.SubProductId);

		subProductDto = new ExtendedSubProductDto(subProduct);
		logger.LogInformation("Got subproduct {SubProductId}", subProduct.Id);
	}

	public override async Task PostTransaction(ConsumeContext<GetSubProductOrder> context)
	{
		await RespondAsync(context, new GetSubProductResponse() 
		{
			SubProduct = subProductDto
		});
	}
}
