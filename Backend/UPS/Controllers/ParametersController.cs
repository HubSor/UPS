using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using UPS.Attributes;
using Services.Application;
using Messages.Commands;
using Messages.Responses;

namespace UPS.Controllers
{
	[Route(template: "parameters")]
	public class ParametersController : BaseController
	{
		public ParametersController(ICqrsService cqrsService)
			: base(cqrsService)
		{
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> Add([FromBody] AddParameterOrder order)
		{
			return await PerformCommand<AddParameterOrder, AddParameterResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditParameterOrder order)
		{
			return await PerformCommand<EditParameterOrder, EditParameterResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteParameterOrder order)
		{
			return await PerformCommand<DeleteParameterOrder, DeleteParameterResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("options/add")]
		public async Task<IActionResult> AddOption([FromBody] AddOptionOrder order)
		{
			return await PerformCommand<AddOptionOrder, AddOptionResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("options/delete")]
		public async Task<IActionResult> DeleteOption([FromBody] DeleteOptionOrder order)
		{
			return await PerformCommand<DeleteOptionOrder, DeleteOptionResponse>(order);
		}
	}
}
