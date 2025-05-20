using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using UPS.Attributes;
using Services.Application;
using Messages.Commands;
using Messages.Responses;
using Messages.Queries;

namespace UPS.Controllers
{
	[Route("sales")]
	public class SalesController : BaseController
	{
		public SalesController(ICqrsService cqrsService)
			: base(cqrsService)
		{
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("save")]
		public async Task<IActionResult> SaveSale([FromBody] SaveSaleOrder order)
		{
			return await PerformCommand<SaveSaleOrder, SaveSaleResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("list")]
		public async Task<IActionResult> ListSales([FromBody] ListSalesQuery order)
		{
			return await PerformQuery<ListSalesQuery, ListSalesResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("get")]
		public async Task<IActionResult> GetSale([FromBody] GetSaleQuery order)
		{
			return await PerformQuery<GetSaleQuery, GetSaleResponse>(order);
		}
	}
}
