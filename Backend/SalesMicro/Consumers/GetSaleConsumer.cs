using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using Core.Web;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace SalesMicro.Consumers;

public class GetSaleConsumer(
	ILogger<GetSaleConsumer> _logger,
	IRepository<Sale> sales,
	IUnitOfWork unitOfWork,
	IRequestClient<GetSaleParametersOrder> getSaleParamsClient
) : TransactionConsumer<GetSaleOrder, GetSaleResponse>(unitOfWork, _logger)
{
    private SaleDetailsDto saleDetailsDto = default!;

    public override async Task<bool> PreTransaction(ConsumeContext<GetSaleOrder> context)
	{
		var sale = await sales.GetAll().FirstAsync(p => p.Id == context.Message.SaleId);
		if (sale == null)
		{
			await RespondWithValidationFailAsync(context, "SaleId", "Nie znaleziono transakcji");
			return false;
		}

		var saleParamsResponse = await getSaleParamsClient.GetResponse<ApiResponse<GetSaleParametersResponse>>(new GetSaleParametersOrder(sale.Id, sale.ProductId));
		if (!saleParamsResponse.Message.Success)
		{
			await RespondWithValidationFailAsync(context, "SaleId", "Nie znaleziono transakcji");
			return false;
		}

		var subProductTax = saleParamsResponse.Message.Data!.SubProducts.Select(x => x.Tax).Sum();
		saleDetailsDto = new SaleDetailsDto()
		{
			SaleId = sale.Id,
			SaleTime = sale.SaleTime.ToString("dd/MM/yyyy HH:mm"),
			TotalPrice = sale.FinalPrice,
			ProductPrice = sale.ProductPrice,
			ProductTax = sale.ProductTax,
			ClientId = sale.ClientId,
			ProductCode = sale.ProductCode,
			ProductId = sale.ProductId,
			ClientName = sale.ClientName,
			SubProductCodes = sale.SubProductCodes,
			TotalTax = subProductTax + sale.ProductTax,
			Parameters = saleParamsResponse.Message.Data!.Parameters,
		};

		return true;
	}

	public override Task InTransaction(ConsumeContext<GetSaleOrder> context)
	{
		logger.LogInformation("Got sale {SaleId} details", saleDetailsDto.SaleId);
		return Task.CompletedTask;
	}

	public override async Task PostTransaction(ConsumeContext<GetSaleOrder> context)
	{
		await RespondAsync(context, new GetSaleResponse()
		{
			SaleDetailsDto = saleDetailsDto
		});
	}
}
