using Microsoft.AspNetCore.Mvc;
using Services.Application;
using FluentValidation;

namespace UPS.Controllers
{
	[ApiController]
	public abstract class BaseController(IServiceProvider sp) : ControllerBase
	{
		protected async Task<IActionResult> PerformAction<O, R>(O order, IBaseApplicationService appService, Func<Task<R>> action)
			where O : class
			where R : class
		{
			var validators = sp.GetServices<IValidator<O>>();
			var resp = await appService.PerformRequestAsync((o) => action(), order, validators);

			return new ObjectResult(resp)
			{
				StatusCode = (int)resp.StatusCode
			};
		}
	}
}
