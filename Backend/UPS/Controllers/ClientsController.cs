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
		[Route("/people/find")]
		public async Task<IActionResult> FindPerson([FromBody] FindPersonClientOrder order)
		{
			return await RespondAsync<FindPersonClientOrder, FindPersonClientResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("/companies/find")]
		public async Task<IActionResult> FindCompany([FromBody] FindCompanyClientOrder order)
		{
			return await RespondAsync<FindCompanyClientOrder, FindCompanyClientResponse>(order);
		}
	}
}
