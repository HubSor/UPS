using Core;
using Data;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class AddSubProductConsumer : TransactionConsumer<AddSubProductOrder, AddSubProductResponse>
{
	private readonly IRepository<SubProduct> subProducts;
	
	public AddSubProductConsumer(ILogger<AddSubProductConsumer> logger, IRepository<SubProduct> subProducts, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.subProducts = subProducts;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<AddSubProductOrder> context)
	{
		if (await subProducts.GetAll().AnyAsync(x => x.Code == context.Message.Code.ToUpper()))
		{
			await RespondWithValidationFailAsync(context, "Code", "Istnieje już podprodukt o takim kodzie");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<AddSubProductOrder> context)
	{
		var subProduct = new SubProduct()
		{
			Name = context.Message.Name,
			Code = context.Message.Code.ToUpper(),
			Description = context.Message.Description,
			BasePrice = context.Message.BasePrice,
		};
		
		await subProducts.AddAsync(subProduct);
	}

	public override async Task PostTransaction(ConsumeContext<AddSubProductOrder> context)
	{
		await RespondAsync(context, new AddSubProductResponse());
	}
}
