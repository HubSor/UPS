using Core;
using Dtos;
using Dtos.Products;
using MassTransit;
using Messages.Queries;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Query;
public class ListProductsConsumer : BaseQueryConsumer<ListProductsQuery, ListProductsResponse>
{
	private readonly IReadRepository<Product> products;
	
	public ListProductsConsumer(ILogger<ListProductsConsumer> logger, IReadRepository<Product> products)
		: base(logger)
	{
		this.products = products;
	}

	public override async Task Consume(ConsumeContext<ListProductsQuery> context)
	{
		var query = products.GetAll().Where(x => context.Message.Statuses.Contains(x.Status) && !x.Deleted);
		
		var totalCount = await query.CountAsync();
		var dtos = await query
			.OrderBy(x => x.Id)
			.Skip(context.Message.Pagination.PageIndex * context.Message.Pagination.PageSize)
			.Take(context.Message.Pagination.PageSize)
			.Select(p => new ProductDto(p))
			.ToListAsync();
			
		logger.LogInformation("Listed products");
		
		var response = new ListProductsResponse()
		{
			Products = new PagedList<ProductDto>(dtos, totalCount, context.Message.Pagination.PageIndex, context.Message.Pagination.PageSize)	
		};

		await RespondAsync(context, response);
	}
}
