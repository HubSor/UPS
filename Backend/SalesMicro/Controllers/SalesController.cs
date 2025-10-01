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
			return await RespondAsync<SaveSaleOrder, SaveSaleResponse>(order);
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
                ProductsMicroUrl + "/parameters/sale",
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
