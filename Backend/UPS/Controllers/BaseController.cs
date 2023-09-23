using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Core;

namespace UPS.Controllers
{
	[ApiController]
	public abstract class BaseController : ControllerBase
	{
		protected IMediator Mediator { get; set; }
		public BaseController(IMediator mediator)
		{
			Mediator = mediator;
		}

		protected async Task<IActionResult> RespondAsync<O, R>(O order)
			where O : class
			where R : class
		{
			var client = Mediator.CreateRequestClient<O>();
			var response = await client.GetResponse<ApiResponse<R>>(order);
            return new ObjectResult(response.Message)
            {
                StatusCode = (int)response.Message.StatusCode
            };
		}
	}
}
