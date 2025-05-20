using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using UPS.Attributes;
using Services.Application;
using Messages.Commands;
using Messages.Responses;
using Messages.Queries;

namespace UPS.Controllers
{
	[Route(template: "clients")]
	public class ClientsController : BaseController
	{
		public ClientsController(ICqrsService cqrsService)
			: base(cqrsService)
		{
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("upsert")]
		public async Task<IActionResult> Upsert([FromBody] UpsertClientOrder order)
		{
			return await PerformCommand<UpsertClientOrder, UpsertClientResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("people/find")]
		public async Task<IActionResult> FindPerson([FromBody] FindPersonClientQuery order)
		{
			return await PerformQuery<FindPersonClientQuery, FindPersonClientResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Administrator)]
		[Route("people/list")]
		public async Task<IActionResult> ListPeople([FromBody] ListPersonClientsQuery order)
		{
			return await PerformQuery<ListPersonClientsQuery, ListPersonClientsResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("companies/find")]
		public async Task<IActionResult> FindCompany([FromBody] FindCompanyClientQuery order)
		{
			return await PerformQuery<FindCompanyClientQuery, FindCompanyClientResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Administrator)]
		[Route("companies/list")]
		public async Task<IActionResult> ListCompanies([FromBody] ListCompanyClientsQuery order)
		{
			return await PerformQuery<ListCompanyClientsQuery, ListCompanyClientsResponse>(order);
		}
	}
}
