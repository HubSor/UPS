using Core;
using Data;
using MassTransit;
using MassTransit.Mediator;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class AddSubProductConsumer : TransactionConsumer<AddSubProductOrder, AddSubProductResponse>
{
	private readonly IRepository<SubProduct> subProducts;
	private readonly IMediator mediator;
	private SubProduct subProduct = default!;
	
	public AddSubProductConsumer(ILogger<AddSubProductConsumer> logger, IRepository<SubProduct> subProducts, IUnitOfWork unitOfWork, IMediator mediator)
		: base(unitOfWork, logger)
	{
		this.subProducts = subProducts;
		this.mediator = mediator;
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
		subProduct = new SubProduct()
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
		if (context.Message.ProductId.HasValue)
		{
			var order = new AssignSubProductOrder(context.Message.ProductId.Value, subProduct.Id, subProduct.BasePrice);
			try
			{
				var client = mediator.CreateRequestClient<AssignSubProductOrder>();
				var response = await client.GetResponse<ApiResponse<AssignSubProductResponse>>(order);
				
				if (response.Message.Success)
					await RespondAsync(context, new AddSubProductResponse());
				else 
					await context.RespondAsync(ApiResponse<AddSubProductResponse>.FromApiResponse(response.Message));
			}
			catch 
			{
				await RespondWithValidationFailAsync(context, "ProductId", "Powiązanie z produktem nie powiodło się");
				throw;
			}
		}
		else
			await RespondAsync(context, new AddSubProductResponse());
	}
}
