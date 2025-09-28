using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicro.Consumers;

public class ListSubProductsConsumer : TransactionConsumer<ListSubProductsOrder, ListSubProductsResponse>
{
	private readonly IRepository<Product> products;
	private readonly IRepository<SubProduct> subProducts;
	private ListSubProductsResponse response = default!;
	
	public ListSubProductsConsumer(ILogger<ListSubProductsConsumer> logger, IRepository<Product> products,
		IRepository<SubProduct> subProducts, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.products = products;
		this.subProducts = subProducts;
	}
	
	public override async Task<bool> PreTransaction(ConsumeContext<ListSubProductsOrder> context)
	{
		if (context.Message.ProductId.HasValue && !await products.GetAll().AnyAsync(x => x.Id == context.Message.ProductId.Value))
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return false;
		}
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<ListSubProductsOrder> context)
	{
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
			
		response = new ListSubProductsResponse()
		{
			SubProducts = new PagedList<SubProductDto>(dtos, totalCount, context.Message.Pagination.PageIndex, context.Message.Pagination.PageSize)
		};
		logger.LogInformation("Listed subproducts");
	}

	public override async Task PostTransaction(ConsumeContext<ListSubProductsOrder> context)
	{
		await RespondAsync(context, response);
	}
}
