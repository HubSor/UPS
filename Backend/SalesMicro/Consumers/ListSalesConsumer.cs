using System.Globalization;
using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace SalesMicro.Consumers;

public class ListSalesConsumer : BaseConsumer<ListSalesOrder, ListSalesResponse>
{
	private readonly IRepository<Sale> sales;
	
	public ListSalesConsumer(ILogger<ListSalesConsumer> logger, IRepository<Sale> sales)
		: base(logger)
	{
		this.sales = sales;
	}

	private static PersonClientDto? GetPersonClient(Client? client) => client is PersonClient personClient ?
		personClient.ToDto() :
		null;

	private static CompanyClientDto? GetCompanyClient(Client? client) => client is CompanyClient companyClient ?
		companyClient.ToDto() :
		null;

	public override async Task Consume(ConsumeContext<ListSalesOrder> context)
	{
		var saleCount = sales.GetAll().Count();
		var saleList = await sales.GetAll()
			.OrderByDescending(x => x.Id)
			.Include(x => x.Client)
			.Include(x => x.Product)
			.Include(x => x.SubProducts)
			.ThenInclude(x => x.SubProduct)
			.Skip(context.Message.Pagination.PageIndex * context.Message.Pagination.PageSize)
			.Take(context.Message.Pagination.PageSize)
			.Select(s => new SaleDto()
			{
				SaleId = s.Id,
				SaleTime = s.SaleTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
				ProductCode = s.Product.Code,
				SubProductCodes = s.SubProducts.Select(x => x.SubProduct.Code).ToList(),
				PersonClient = GetPersonClient(s.Client),
				CompanyClient = GetCompanyClient(s.Client),
				TotalPrice = s.FinalPrice,
			})
			.ToListAsync();
			
		await RespondAsync(context, new ListSalesResponse()
		{
			Sales = new PagedList<SaleDto>(saleList, saleCount,
				context.Message.Pagination.PageIndex, context.Message.Pagination.PageSize)
		});
		logger.LogInformation("Listed sales");
	}
}
