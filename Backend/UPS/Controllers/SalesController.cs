using Microsoft.AspNetCore.Mvc;
using Core.Web;
using Core.Models;
using Core.Messages;

namespace UPS.Controllers
{
	[Route("sales")]
	public class SalesController : BaseController
	{
		protected Random rnd = new();
		protected string[] MicroUrls => (Environment.GetEnvironmentVariable("SALES_URL") ?? "https://localhost:2110").Split(";");
		protected override string TargetMicroUrl => MicroUrls[rnd.Next(MicroUrls.Length)];
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("save")]
		public async Task<IActionResult> SaveSale([FromBody] SaveSaleOrder order)
		{
			return await RelayMessage(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("list")]
		public async Task<IActionResult> ListSales([FromBody] ListSalesOrder order)
		{
			return await RelayMessage(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("get")]
		public async Task<IActionResult> GetSale([FromBody] GetSaleOrder order)
		{
			return await RelayMessage(order);
		}
	}
}
