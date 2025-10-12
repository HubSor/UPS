using Microsoft.AspNetCore.Mvc;
using Core.Web;
using Core.Models;
using Core.Messages;
using MassTransit.Mediator;

namespace ProductsMicro.Controllers
{
	[Route(template: "sales")]
	public class SalesController(IMediator mediator) : BaseMediatorController(mediator)
	{
		[HttpPost]
		[AuthorizeRoles(RoleEnum.SaleManager, RoleEnum.Administrator)]
		[Route("parameters")]
		public async Task<IActionResult> GetSaleParameters([FromBody] GetSaleParametersOrder order)
		{
			return await RespondAsync<GetSaleParametersOrder, GetSaleParametersResponse>(order);
		}
	}
}
