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
            var resp = await GetApiResponse<TOrder, TResponse>(order);
            return new ObjectResult(resp)
            {
                StatusCode = (int)resp.StatusCode
            };
        }
        
        protected async Task<ApiResponse<TResponse>> GetApiResponse<TOrder, TResponse>(TOrder order)
            where TOrder : BaseOrder
            where TResponse : class
        {
            var claims = HttpContext.User.Claims ?? [];
            order.Claims = [..
                claims.Select(x => new ClaimDto(x))
            ];

            var client = sp.GetRequiredService<IRequestClient<TOrder>>();
            var mtResponse = await client.GetResponse<ApiResponse<TResponse>>(order);
            return mtResponse.Message;
        }
    }
}
