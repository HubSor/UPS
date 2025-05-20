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
public class ListSubProductsConsumer : BaseQueryConsumer<ListSubProductsQuery, ListSubProductsResponse>
{
	private readonly IRepository<Product> products;
	private readonly IRepository<SubProduct> subProducts;
	
	public ListSubProductsConsumer(
		ILogger<ListSubProductsConsumer> logger,
		IRepository<Product> products,
		IRepository<SubProduct> subProducts
	) : base(logger)
	{
		this.products = products;
		this.subProducts = subProducts;
	}
	
	public override async Task Consume(ConsumeContext<ListSubProductsQuery> context)
	{
		if (context.Message.ProductId.HasValue && !await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductId.Value))
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return;
		}

		var query = subProducts.GetAll().Where(sp => !sp.Deleted);
		if (context.Message.ProductId.HasValue)
		{
			query = query
				.Include(s => s.SubProductInProducts)
				.Where(s => !s.SubProductInProducts.Any(sp => sp.ProductId == context.Message.ProductId));
		}
		
		var totalCount = await query.CountAsync();
		
		var dtos = (await query.ToListAsync())
			.OrderBy(x => x.Id)
			.Skip(context.Message.Pagination.PageIndex * context.Message.Pagination.PageSize)
			.Take(context.Message.Pagination.PageSize)
			.Select(p => new SubProductDto(p))
			.ToList();
			
		logger.LogInformation("Listed subproducts");
		var response = new ListSubProductsResponse()
		{
			SubProducts = new PagedList<SubProductDto>(dtos, totalCount, context.Message.Pagination.PageIndex, context.Message.Pagination.PageSize)
		};

		await RespondAsync(context, response);
	}
}
