using Microsoft.AspNetCore.Mvc;
using WebCommons;
using Core.Web;
using Core.Models;
using Core.Messages;
using MassTransit.Mediator;

namespace ProductsMicro.Controllers
{
	[Route(template: "parameters")]
	public class ParametersController(IMediator mediator) : BaseMediatorController(mediator)
	{
		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> Add([FromBody] AddParameterOrder order)
		{
			return await RespondAsync<AddParameterOrder, AddParameterResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditParameterOrder order)
		{
			return await RespondAsync<EditParameterOrder, EditParameterResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteParameterOrder order)
		{
			return await RespondAsync<DeleteParameterOrder, DeleteParameterResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("options/add")]
		public async Task<IActionResult> AddOption([FromBody] AddOptionOrder order)
		{
			return await RespondAsync<AddOptionOrder, AddOptionResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("options/delete")]
		public async Task<IActionResult> DeleteOption([FromBody] DeleteOptionOrder order)
		{
			return await RespondAsync<DeleteOptionOrder, DeleteOptionResponse>(order);
		}
	}
}
