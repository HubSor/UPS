using Microsoft.AspNetCore.Mvc;
using MassTransit.Mediator;
using Messages.Sales;
using Models.Entities;
using UPS.Attributes;

namespace UPS.Controllers
{
	[Route("sales")]
	public class SalesController : BaseController
	{
		public SalesController(IMediator mediator)
			: base(mediator)
		{
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("save")]
		public async Task<IActionResult> SaveSale([FromBody] SaveSaleOrder order)
		{
			return await RespondAsync<SaveSaleOrder, SaveSaleResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.Administrator)]
		[Route("list")]
		public async Task<IActionResult> ListSales([FromBody] ListSalesOrder order)
		{
			return await RespondAsync<ListSalesOrder, ListSalesResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.Administrator)]
		[Route("get")]
		public async Task<IActionResult> GetSale([FromBody] GetSaleOrder order)
		{
			return await RespondAsync<GetSaleOrder, GetSaleResponse>(order);
		}
	}
}
