using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace SalesMicro.Consumers;

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
			.FirstAsync(p => p.Id == context.Message.SaleId);

		saleDetailsDto = new SaleDetailsDto()
		{
			SaleId = sale.Id,
			SaleTime = sale.SaleTime.ToString("dd/MM/yyyy HH:mm"),
			TotalPrice = sale.FinalPrice,
			ProductPrice = sale.ProductPrice,
			ProductTax = sale.ProductTax,
			ClientId = sale.ClientId,
		};

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
