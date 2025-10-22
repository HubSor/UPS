using Core.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Web
{
	[ApiController]
	public abstract class BaseController(IServiceProvider sp) : ControllerBase
	{
		protected async Task<IActionResult> RespondAsync<TOrder, TResponse>(TOrder order)
            where TOrder : BaseOrder
            where TResponse : class
        {
            var claims = HttpContext.User.Claims ?? [];
            order.Claims = [..
                claims.Select(x => new ClaimDto()
                {
                    Name = x.Type,
                    Value = x.Value 
                })
            ];

            var client = sp.GetRequiredService<IRequestClient<TOrder>>();
            var response = await client.GetResponse<ApiResponse<TResponse>>(order);

            return new ObjectResult(response.Message)
            {
                StatusCode = (int)response.Message.StatusCode
            };
        }
    }
}
