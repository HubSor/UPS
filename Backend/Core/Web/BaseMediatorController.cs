using Microsoft.AspNetCore.Mvc;
using MassTransit.Mediator;
using System.Net.Http.Json;

namespace Core.Web
{
	[ApiController]
	public abstract class BaseMediatorController(IMediator mediator) : ControllerBase
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

		protected async Task<ApiResponse<TResponse>?> MakeMicroRequest<TOrder, TResponse>(string url, TOrder order)
			where TResponse : class
        {
            var msg = new HttpRequestMessage(
				HttpMethod.Parse(HttpContext.Request.Method),
				url
			)
            {
                Content = JsonContent.Create(order),
            };

            if (HttpContext.Request.Headers.TryGetValue("Cookie", out var cookies))
            {
                msg.Headers.TryAddWithoutValidation("Cookie", cookies.FirstOrDefault());
            }

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            };
            var client = new HttpClient(handler);
			var resp = await client.SendAsync(msg);
			return await resp.Content.ReadFromJsonAsync<ApiResponse<TResponse>>();
        }
	}
}