using Microsoft.AspNetCore.Mvc;
using Core.Web;
using Core.Models;
using Core.Messages;

namespace UPS.Controllers
{
	[Route(template: "clients")]
	public class ClientsController : BaseController
	{
		protected override string TargetMicroUrl => Environment.GetEnvironmentVariable("CLIENTS_URL") ?? "https://localhost:2108";

        [HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("upsert")]
		public async Task<IActionResult> Upsert([FromBody] UpsertClientOrder order)
		{
			return await RelayMessage(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("people/find")]
		public async Task<IActionResult> FindPerson([FromBody] FindPersonClientOrder order)
		{
			return await RelayMessage(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Administrator)]
		[Route("people/list")]
		public async Task<IActionResult> ListPeople([FromBody] ListPersonClientsOrder order)
		{
			return await RelayMessage(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Seller, RoleEnum.Administrator)]
		[Route("companies/find")]
		public async Task<IActionResult> FindCompany([FromBody] FindCompanyClientOrder order)
		{
			return await RelayMessage(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ClientManager, RoleEnum.Administrator)]
		[Route("companies/list")]
		public async Task<IActionResult> ListCompanies([FromBody] ListCompanyClientsOrder order)
		{
			return await RelayMessage(order);
		}
	}
}
