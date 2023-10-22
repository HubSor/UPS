using Core;
using Data;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class AddProductConsumer : TransactionConsumer<AddProductOrder, AddProductResponse>
{
	private readonly IRepository<Product> products;
	
	public AddProductConsumer(ILogger<AddProductConsumer> logger, IRepository<Product> products, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.products = products;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<AddProductOrder> context)
	{
		if (await products.GetAll().AnyAsync(x => x.Code == context.Message.Code.ToUpper()))
		{
			await RespondWithValidationFailAsync(context, "Code", "Istnieje już produkt o takim kodzie");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<AddProductOrder> context)
	{
		var product = new Product()
		{
			Name = context.Message.Name,
			Code = context.Message.Code.ToUpper(),
			Description = context.Message.Description,
			BasePrice = context.Message.BasePrice,
			AnonymousSaleAllowed = context.Message.AnonymousSaleAllowed,
			Status = ProductStatusEnum.NotOffered,
		};
		
		await products.AddAsync(product);
	}

	public override async Task PostTransaction(ConsumeContext<AddProductOrder> context)
	{
		await RespondAsync(context, new AddProductResponse());
	}
}
