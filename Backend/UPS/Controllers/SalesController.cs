using Microsoft.AspNetCore.Mvc;
using Messages.Sales;
using Models.Entities;
using UPS.Attributes;
using Services.Application;

namespace UPS.Controllers
{
	[Route("sales")]
	public class SalesController(IServiceProvider sp, ISalesApplicationService salesApplicationService) : BaseController(sp)
	{
        [HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("save")]
		public async Task<IActionResult> SaveSale([FromBody] SaveSaleOrder order)
		{
			return await PerformAction(order, salesApplicationService, () => salesApplicationService.SaveSaleAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("list")]
		public async Task<IActionResult> ListSales([FromBody] ListSalesOrder order)
		{
			return await PerformAction(order, salesApplicationService, () => salesApplicationService.ListSalesAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("get")]
		public async Task<IActionResult> GetSale([FromBody] GetSaleOrder order)
		{
			return await PerformAction(order, salesApplicationService, () => salesApplicationService.GetSaleAsync(order));
		}
	}
}
