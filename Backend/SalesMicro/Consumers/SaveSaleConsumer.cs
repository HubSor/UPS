using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using Core.Web;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace SalesMicro.Consumers;

public class SaveSaleConsumer(
	ILogger<SaveSaleConsumer> _logger,
	IRepository<Sale> sales,
	IUnitOfWork unitOfWork,
	IRequestClient<GetClientOrder> getClientClient,
	IRequestClient<GetProductOrder> getProductClient,
	IRequestClient<SaveSaleProductsMicroOrder> saveSaleClient
) : TransactionConsumer<SaveSaleOrder, SaveSaleResponse>(unitOfWork, _logger)
{
	private int _saleId;
	private string _clientName = "";
	private ExtendedProductDto _productDto = default!;

    public override async Task<bool> PreTransaction(ConsumeContext<SaveSaleOrder> context)
	{
		if (context.Message.ClientId.HasValue)
		{
			var clientResponse = await getClientClient.GetResponse<ApiResponse<GetClientResponse>>(new GetClientOrder(context.Message.ClientId.Value));
			if (!clientResponse.Message.Success)
			{
				await RespondWithValidationFailAsync(context, "ClientId", "Nie znaleziono klienta");
				return false;
			}

			_clientName = clientResponse.Message.Data!.CompanyClient?.CompanyName ?? clientResponse.Message.Data.PersonClient?.GetName() ?? "";
		}

		var productResponse = await getProductClient.GetResponse<ApiResponse<GetProductResponse>>(new GetProductOrder(context.Message.ProductId));
		if (!productResponse.Message.Success)
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return false;
		}

		_productDto = productResponse.Message.Data!.Product;
		return true;
    }

	public override async Task InTransaction(ConsumeContext<SaveSaleOrder> context)
	{
		var totalPrice = context.Message.ProductPrice + context.Message.SubProducts.Select(s => s.Price).Sum();
		var selectedSubProducts = _productDto.SubProducts.Where(s => context.Message.SubProducts.Any(sp => sp.SubProductId == s.Id));

		var newSale = new Sale
		{
			ClientId = context.Message.ClientId,
			ProductId = context.Message.ProductId,
			ProductCode = _productDto.Code,
			SubProductCodes = string.Join(',', selectedSubProducts.Select(x => x.Code)),
			ClientName = _clientName,
			SellerId = context.Message.GetUserId(),
			FinalPrice = totalPrice,
			ProductPrice = context.Message.ProductPrice,
			ProductTax = context.Message.ProductPrice * _productDto.TaxRate,
			SaleTime = DateTime.Now
		};

		await sales.AddAsync(newSale);

		logger.LogInformation("Saved new sale {SaleId}", newSale.Id);

		_saleId = newSale.Id;
	}

	public override async Task PostTransaction(ConsumeContext<SaveSaleOrder> context)
	{
		await RespondAsync(context, new SaveSaleResponse()
        {
            SaleId = _saleId,
        });

		await saveSaleClient.GetResponse<ApiResponse<SaveSaleResponse>>(new SaveSaleProductsMicroOrder(
			_saleId,
			context.Message.Answers,
			context.Message.SubProducts
		));
	}
}