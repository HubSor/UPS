using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Web
{
	[ApiController]
	public abstract class BaseController(IServiceProvider sp) : ControllerBase
	{
		protected async Task<IActionResult> RespondAsync<TOrder, TResponse>(TOrder order)
            where TOrder : class
            where TResponse : class
        {
            var client = sp.GetRequiredService<IRequestClient<TOrder>>();
            var response = await client.GetResponse<ApiResponse<TResponse>>(order);

            return new ObjectResult(response.Message)
            {
                StatusCode = (int)response.Message.StatusCode
            };
        }
    }
}
