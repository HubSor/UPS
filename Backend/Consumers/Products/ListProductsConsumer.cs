using Core;
using Data;
using Dtos.Products;
using MassTransit;
using Messages.Products;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Products;
public class ListProductsConsumer : TransactionConsumer<ListProductsOrder, ListProductsResponse>
{
	private readonly IRepository<Product> products;
	private ListProductsResponse response = default!;
	
	public ListProductsConsumer(ILogger<ListProductsConsumer> logger, IRepository<Product> products, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.products = products;
	}

	public override Task InTransaction(ConsumeContext<ListProductsOrder> context)
	{
		var dtos = products.GetAll().Where(x => context.Message.Statuses.Contains(x.Status))
			.Select(p => new ProductDto() 
			{
				Id = p.Id,
				Name = p.Name,
				Description = p.Description,
				Code = p.Code,
				Status = p.Status,
				AnonymousSaleAllowed = p.AnonymousSaleAllowed,
				BasePrice = p.BasePrice
			}).ToList();
			
		response = new ListProductsResponse()
		{
			Products = dtos	
		};
		
		return Task.CompletedTask;
	}

	public override async Task PostTransaction(ConsumeContext<ListProductsOrder> context)
	{
		await RespondAsync(context, response);
	}
}
