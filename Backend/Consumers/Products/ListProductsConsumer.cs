using Core;
using Data;
using Dtos;
using Dtos.Products;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
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

	public override async Task InTransaction(ConsumeContext<ListProductsOrder> context)
	{
		var query = products.GetAll().Where(x => context.Message.Statuses.Contains(x.Status) && !x.Deleted);
		
		var totalCount = await query.CountAsync();
		var dtos = await query
			.OrderBy(x => x.Id)
			.Skip(context.Message.Pagination.PageIndex * context.Message.Pagination.PageSize)
			.Take(context.Message.Pagination.PageSize)
			.Select(p => new ProductDto(p))
			.ToListAsync();
			
		response = new ListProductsResponse()
		{
			Products = new PagedList<ProductDto>(dtos, totalCount, context.Message.Pagination.PageIndex, context.Message.Pagination.PageSize)	
		};
		logger.LogInformation("Listed products");
	}

	public override async Task PostTransaction(ConsumeContext<ListProductsOrder> context)
	{
		await RespondAsync(context, response);
	}
}
