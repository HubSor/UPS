using Microsoft.AspNetCore.Mvc;
using Services.Application;
using Messages.Queries;
using Messages.Commands;

namespace UPS.Controllers
{
	[ApiController]
	public abstract class BaseController : ControllerBase
	{
		protected ICqrsService CqrsService { get; set; }

		public BaseController(ICqrsService cqrsService)
		{
			CqrsService = cqrsService;
		}

		protected async Task<IActionResult> PerformQuery<Q, R>(Q query)
			where Q : Query
			where R : class
		{
			var response = await CqrsService.PerformQueryAsync<Q, R>(query);
            return new ObjectResult(response)
            {
                StatusCode = (int)response.StatusCode
            };
		}

		protected async Task<IActionResult> PerformCommand<C, R>(C command)
			where C : Command
			where R : class
		{
			var response = await CqrsService.PerformCommandAsync<C, R>(command);
			return new ObjectResult(response)
			{
				StatusCode = (int)response.StatusCode
			};
		}
	}
}
