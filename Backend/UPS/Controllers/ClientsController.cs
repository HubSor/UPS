using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using UPS.Attributes;
using Messages.Clients;
using Services.Application;

namespace UPS.Controllers
{
	[Route(template: "clients")]
	public class ClientsController(IServiceProvider sp, IClientsApplicationService clientsApplicationService) : BaseController(sp)
	{
        [HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("upsert")]
		public async Task<IActionResult> Upsert([FromBody] UpsertClientOrder order)
		{
			return await PerformAction(order, clientsApplicationService, () => clientsApplicationService.UpsertClientAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("people/find")]
		public async Task<IActionResult> FindPerson([FromBody] FindPersonClientOrder order)
		{
			return await PerformAction(order, clientsApplicationService, () => clientsApplicationService.FindPersonAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Administrator)]
		[Route("people/list")]
		public async Task<IActionResult> ListPeople([FromBody] ListPersonClientsOrder order)
		{
			return await PerformAction(order, clientsApplicationService, () => clientsApplicationService.ListPeopleAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("companies/find")]
		public async Task<IActionResult> FindCompany([FromBody] FindCompanyClientOrder order)
		{
			return await PerformAction(order, clientsApplicationService, () => clientsApplicationService.FindCompanyAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Administrator)]
		[Route("companies/list")]
		public async Task<IActionResult> ListCompanies([FromBody] ListCompanyClientsOrder order)
		{
			return await PerformAction(order, clientsApplicationService, () => clientsApplicationService.ListCompaniesAsync(order));
		}
	}
}
