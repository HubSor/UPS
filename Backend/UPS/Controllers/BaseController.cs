using Microsoft.AspNetCore.Mvc;
using Services.Application;
using Messages.Queries;

namespace UPS.Controllers
{
	[ApiController]
	public abstract class BaseController : ControllerBase
	{
		protected IQueryService QueryService { get; set; }

		public BaseController(IQueryService queryService)
		{
			QueryService = queryService;
		}

		protected async Task<IActionResult> PerformQuery<Q, R>(Q query)
			where Q : Query
			where R : class
		{
			var response = await QueryService.PerformQueryAsync<Q, R>(query);
            return new ObjectResult(response)
            {
                StatusCode = (int)response.StatusCode
            };
		}
	}
}
