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
		[Route("upsert")]
		public async Task<IActionResult> Upsert([FromBody] UpsertClientOrder order)
		{
			return await RespondAsync<UpsertClientOrder, UpsertClientResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("people/find")]
		public async Task<IActionResult> FindPerson([FromBody] FindPersonClientOrder order)
		{
			return await RespondAsync<FindPersonClientOrder, FindPersonClientResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.Administrator)]
		[Route("people/list")]
		public async Task<IActionResult> ListPeople([FromBody] ListPersonClientsOrder order)
		{
			return await RespondAsync<ListPersonClientsOrder, ListPersonClientsResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("companies/find")]
		public async Task<IActionResult> FindCompany([FromBody] FindCompanyClientOrder order)
		{
			return await RespondAsync<FindCompanyClientOrder, FindCompanyClientResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.Administrator)]
		[Route("companies/list")]
		public async Task<IActionResult> ListCompanies([FromBody] ListCompanyClientsOrder order)
		{
			return await RespondAsync<ListCompanyClientsOrder, ListCompanyClientsResponse>(order);
		}
	}
}
