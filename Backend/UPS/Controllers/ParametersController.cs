using Microsoft.AspNetCore.Mvc;
using MassTransit.Mediator;
using Models.Entities;
using UPS.Attributes;
using Messages.Parameters;

namespace UPS.Controllers
{
	[Route(template: "parameters")]
	public class ParametersController : BaseController
	{
		public ParametersController(IMediator mediator)
			: base(mediator)
		{
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> Add([FromBody] AddParameterOrder order)
		{
			return await PerformQuery<AddParameterOrder, AddParameterResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditParameterOrder order)
		{
			return await PerformQuery<EditParameterOrder, EditParameterResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteParameterOrder order)
		{
			return await PerformQuery<DeleteParameterOrder, DeleteParameterResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("options/add")]
		public async Task<IActionResult> AddOption([FromBody] AddOptionOrder order)
		{
			return await PerformQuery<AddOptionOrder, AddOptionResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("options/delete")]
		public async Task<IActionResult> DeleteOption([FromBody] DeleteOptionOrder order)
		{
			return await PerformQuery<DeleteOptionOrder, DeleteOptionResponse>(order);
		}
	}
}
