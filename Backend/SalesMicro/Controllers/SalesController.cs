using Microsoft.AspNetCore.Mvc;
using Core.Web;
using Core.Models;
using Core.Messages;
using MassTransit.Mediator;

namespace SalesMicro.Controllers
{
	[Route("sales")]
	public class UsersController(IMediator mediator) : BaseMediatorController(mediator)
	{
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
			return await RespondAsync<GetSaleOrder, GetSaleResponse>(order);
		}
	}
}
