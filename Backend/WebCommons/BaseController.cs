using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Core;

namespace WebCommons
{
	[ApiController]
	public abstract class BaseController(IMediator mediator) : ControllerBase
	{
        protected IMediator Mediator { get; set; } = mediator;

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
