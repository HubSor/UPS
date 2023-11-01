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
			return await RespondAsync<AddParameterOrder, AddParameterResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("options/add")]
		public async Task<IActionResult> AddOption([FromBody] AddOptionOrder order)
		{
			return await RespondAsync<AddOptionOrder, AddOptionResponse>(order);
		}
	}
}
