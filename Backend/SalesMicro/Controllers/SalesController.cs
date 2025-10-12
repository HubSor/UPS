using Microsoft.AspNetCore.Mvc;
using Core.Web;
using Core.Models;
using Core.Messages;
using MassTransit.Mediator;
using Core.Dtos;

namespace SalesMicro.Controllers
{
	[Route("sales")]
	public class UsersController(IMediator mediator) : BaseMediatorController(mediator)
	{
		protected const string ClientsMicroUrl = "https://localhost:2108";
		protected const string ProductsMicroUrl = "https://localhost:2109";

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("save")]
		public async Task<IActionResult> SaveSale([FromBody] SaveSaleOrder order)
        {
            var clientName = "";
            if (order.ClientId.HasValue)
            {
                var clientResponse = await MakeMicroRequest<GetClientOrder, GetClientResponse>(ClientsMicroUrl + "/clients/get", new GetClientOrder(order.ClientId.Value));
                if (clientResponse?.Success != true)
                {
                    return new ObjectResult(clientResponse)
                    {
                        StatusCode = 400,
                    };
                }

                clientName = clientResponse.Data!.CompanyClient?.CompanyName ?? clientResponse.Data.PersonClient?.GetName() ?? "";
            }

            var productResponse = await MakeMicroRequest<GetProductOrder, GetProductResponse>(ProductsMicroUrl + "/products/get", new GetProductOrder(order.ProductId));
            if (productResponse?.Success != true)
            {
                return new ObjectResult(productResponse)
                {
                    StatusCode = 400,
                };
            }

			var client = Mediator.CreateRequestClient<ExtendedSaveSaleOrder>();
			var saleResponse = await client.GetResponse<ApiResponse<SaveSaleResponse>>(new ExtendedSaveSaleOrder(
                order.ProductId,
                order.ClientId,
                order.Answers,
                order.ProductPrice,
                order.SubProducts,
                productResponse.Data!.Product,
                clientName
            ));

            if (!saleResponse.Message.Success)
            {
                return new ObjectResult(saleResponse)
                {
                    StatusCode = (int)saleResponse.Message.StatusCode,
                };
            }
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("list")]
		public async Task<IActionResult> ListSales([FromBody] ListSalesOrder order)
		{
			return await RespondAsync<ListSalesOrder, ListSalesResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("get")]
		public async Task<IActionResult> GetSale([FromBody] GetSaleOrder order)
        {
            var client = Mediator.CreateRequestClient<GetSaleOrder>();
            var response = await client.GetResponse<ApiResponse<GetSaleResponse>>(order);

            var saleDto = response.Message.Data?.SaleDetailsDto;
            if (saleDto == null)
            {
                return new ObjectResult(response.Message)
                {
                    StatusCode = (int)response.Message.StatusCode
                };
            }

            await FetchRelatedData(saleDto);

            return new ObjectResult(response.Message)
            {
                StatusCode = 400,
            };
        }

        private async Task FetchRelatedData(SaleDetailsDto saleDto)
        {
            var paramsResponse = await MakeMicroRequest<GetSaleParametersOrder, GetSaleParametersResponse>(
                ProductsMicroUrl + "/sales/parameters",
                new GetSaleParametersOrder(saleDto.SaleId, saleDto.ProductId)
            );

            if (paramsResponse != null && paramsResponse.Data != null)
            {
                saleDto.Product = paramsResponse.Data.Product;
                saleDto.SubProducts = paramsResponse.Data.SubProducts;
                saleDto.Parameters = paramsResponse.Data.Parameters;
            }

            if (saleDto.ClientId.HasValue)
            {
                var clientResponse = await MakeMicroRequest<GetClientOrder, GetClientResponse>(ClientsMicroUrl + "/clients/get", new GetClientOrder(saleDto.ClientId.Value));
                if (clientResponse != null && clientResponse.Data != null)
                {
                    saleDto.PersonClient = clientResponse.Data.PersonClient;
                    saleDto.CompanyClient = clientResponse.Data.CompanyClient;
                }
            }
        }
    }
}
