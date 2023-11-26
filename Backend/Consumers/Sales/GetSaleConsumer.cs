using Core;
using Data;
using Dtos.Sales;
using MassTransit;
using Messages.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Sales;
public class GetSaleConsumer : TransactionConsumer<GetSaleOrder, GetSaleResponse>
{
	private readonly IRepository<Sale> sales;
	private SaleDetailsDto saleDetailsDto = default!;
	
	public GetSaleConsumer(ILogger<GetSaleConsumer> logger, IRepository<Sale> sales, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.sales = sales;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<GetSaleOrder> context)
	{
		if (!await sales.GetAll().AnyAsync(x => x.Id == context.Message.SaleId))
		{
			await RespondWithValidationFailAsync(context, "SaleId", "Nie znaleziono transakcji");
			return false;
		}

		return true;
	}

	public override async Task InTransaction(ConsumeContext<GetSaleOrder> context)
	{
		var sale = await sales.GetAll()
			.Include(p => p.Client)
			.Include(x => x.Product)
			.Include(x => x.SubProducts)
			.ThenInclude(x => x.SubProduct)
			.Include(x => x.SaleParameters)
			.ThenInclude(x => x.Parameter)
			.ThenInclude(x => x.Options)
			.FirstAsync(p => p.Id == context.Message.SaleId);

		saleDetailsDto = new SaleDetailsDto(sale);
		logger.LogInformation("Got sale {SaleId} details", sale.Id);
	}

	public override async Task PostTransaction(ConsumeContext<GetSaleOrder> context)
	{
		await RespondAsync(context, new GetSaleResponse()
		{
			SaleDetailsDto = saleDetailsDto
		});
	}
}
