using Core;
using Core.Data;
using Core.Messages;
using Core.Models;
using Core.Web;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace SalesMicro.Consumers;

public class SaveSaleConsumer(
	ILogger<SaveSaleConsumer> _logger,
	Repository<Sale> sales,
	IUnitOfWork unitOfWork,
	IHttpContextAccessor httpContextAccessor
) : TransactionConsumer<ExtendedSaveSaleOrder, SaveSaleResponse>(unitOfWork, _logger)
{
	public override async Task InTransaction(ConsumeContext<ExtendedSaveSaleOrder> context)
	{
		var totalPrice = context.Message.ProductPrice + context.Message.SubProducts.Select(s => s.Price).Sum();
		var selectedSubProducts = context.Message.ProductDto.SubProducts.Where(s => context.Message.SubProducts.Any(sp => sp.SubProductId == s.Id));

		var newSale = new Sale
		{
			ClientId = context.Message.ClientId,
			ProductId = context.Message.ProductId,
			ProductCode = context.Message.ProductDto.Code,
			SubProductCodes = string.Join(',', selectedSubProducts.Select(x => x.Code)),
			ClientName = context.Message.ClientName,
			SellerId = httpContextAccessor.GetUserId(),
			FinalPrice = totalPrice,
			ProductPrice = context.Message.ProductPrice,
			ProductTax = context.Message.ProductPrice * context.Message.ProductDto.TaxRate,
			SaleTime = DateTime.Now
		};

		await sales.AddAsync(newSale);

		logger.LogInformation("Saved new sale {SaleId}", newSale.Id);
	}

	public override async Task PostTransaction(ConsumeContext<ExtendedSaveSaleOrder> context)
	{
		await RespondAsync(context, new SaveSaleResponse());
	}
}