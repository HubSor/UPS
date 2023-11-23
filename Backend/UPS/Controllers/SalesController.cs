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
	}
}
