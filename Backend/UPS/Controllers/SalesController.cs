using Microsoft.AspNetCore.Mvc;
using Messages.Sales;
using Models.Entities;
using WebCommons;

namespace UPS.Controllers
{
	[Route("sales")]
	public class SalesController : BaseController
	{
		protected override string TargetMicroUrl => "";
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("save")]
		public async Task<IActionResult> SaveSale([FromBody] SaveSaleOrder order)
		{
			return await RelayMessage();
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("list")]
		public async Task<IActionResult> ListSales([FromBody] ListSalesOrder order)
		{
			return await RelayMessage();
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("get")]
		public async Task<IActionResult> GetSale([FromBody] GetSaleOrder order)
		{
			return await RelayMessage();
		}
	}
}
