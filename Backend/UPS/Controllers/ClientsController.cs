using Microsoft.AspNetCore.Mvc;
using MassTransit.Mediator;
using Models.Entities;
using UPS.Attributes;
using Messages.Clients;

namespace UPS.Controllers
{
	[Route(template: "clients")]
	public class ClientsController : BaseController
	{
		public ClientsController(IMediator mediator)
			: base(mediator)
		{
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("find")]
		public async Task<IActionResult> Find([FromBody] FindClientOrder order)
		{
			return await RespondAsync<FindClientOrder, FindClientResponse>(order);
		}
	}
}
