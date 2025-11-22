using Core;
using Dtos.Sales;
using MassTransit;
using Messages.Queries;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Query;
public class GetSaleConsumer : BaseQueryConsumer<GetSaleQuery, GetSaleResponse>
{
	private readonly IReadRepository<Sale> sales;
	
	public GetSaleConsumer(ILogger<GetSaleConsumer> logger, IReadRepository<Sale> sales)
		: base(logger)
	{
		this.sales = sales;
	}

	public override async Task Consume(ConsumeContext<GetSaleQuery> context)
	{
		if (!await sales.GetAll().AnyAsync(x => x.Id == context.Message.SaleId))
		{
			await RespondWithValidationFailAsync(context, "SaleId", "Nie znaleziono transakcji");
			return;
		}

		var sale = await sales.GetAll()
			.Include(p => p.Client)
			.Include(x => x.Product)
			.Include(x => x.SubProducts)
			.ThenInclude(x => x.SubProduct)
			.Include(x => x.SaleParameters)
			.ThenInclude(x => x.Parameter)
			.ThenInclude(x => x.Options)
			.FirstAsync(p => p.Id == context.Message.SaleId);

		logger.LogInformation("Got sale {SaleId} details", sale.Id);

		var saleDetailsDto = new SaleDetailsDto(sale);
		await RespondAsync(context, new GetSaleResponse()
		{
			SaleDetailsDto = saleDetailsDto
		});
	}
}
