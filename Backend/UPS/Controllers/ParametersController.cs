using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using UPS.Attributes;
using Messages.Parameters;
using Services.Application;

namespace UPS.Controllers
{
	[Route(template: "parameters")]
	public class ParametersController(IServiceProvider sp, IParametersApplicationService parametersApplicationService) : BaseController(sp)
	{
        [HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> Add([FromBody] AddParameterOrder order)
		{
			return await PerformAction(order, parametersApplicationService, () => parametersApplicationService.AddParameterAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditParameterOrder order)
		{
			return await PerformAction(order, parametersApplicationService, () => parametersApplicationService.EditParameterAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteParameterOrder order)
		{
			return await PerformAction(order, parametersApplicationService, () => parametersApplicationService.DeleteParameterAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("options/add")]
		public async Task<IActionResult> AddOption([FromBody] AddOptionOrder order)
		{
			return await PerformAction(order, parametersApplicationService, () => parametersApplicationService.AddOptionAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("options/delete")]
		public async Task<IActionResult> DeleteOption([FromBody] DeleteOptionOrder order)
		{
			return await PerformAction(order, parametersApplicationService, () => parametersApplicationService.DeleteOptionAsync(order));
		}
	}
}
