using System.Globalization;
using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using MassTransit;

namespace SalesMicro.Consumers;

public class ListSalesConsumer(ILogger<ListSalesConsumer> _logger, IRepository<Sale> sales) : BaseConsumer<ListSalesOrder, ListSalesResponse>(_logger)
{
    public override async Task Consume(ConsumeContext<ListSalesOrder> context)
	{
		var saleCount = sales.GetAll().Count();
		var saleList = sales.GetAll()
			.OrderByDescending(x => x.Id)
			.Skip(context.Message.Pagination.PageIndex * context.Message.Pagination.PageSize)
			.Take(context.Message.Pagination.PageSize)
			.ToList()
			.Select(s => new SaleDto()
			{
				SaleId = s.Id,
				SaleTime = s.SaleTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
				ProductCode = s.ProductCode,
				SubProductCodes = s.SubProductCodes == "" ? [] : s.SubProductCodes.Split(";"),
				ClientName = s.ClientName,
				TotalPrice = s.FinalPrice,
			});
			
		await RespondAsync(context, new ListSalesResponse()
		{
			Sales = new PagedList<SaleDto>(saleList, saleCount,
				context.Message.Pagination.PageIndex, context.Message.Pagination.PageSize)
		});

		logger.LogInformation("Listed sales");
	}
}
