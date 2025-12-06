using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Web
{
	[ApiController]
	public abstract class BaseController : ControllerBase
	{
		protected abstract string TargetMicroUrl { get; }

		protected async Task<IActionResult> RelayMessage()
		{
			var original = HttpContext.Request;
			original.Headers.Remove("host");

			var content = new StringContent("");
			return await ForwardContent(content);
		}

		protected async Task<IActionResult> RelayMessage<TOrder>(TOrder order)
        {
            var original = HttpContext.Request;
            original.Headers.Remove("host");

            var content = JsonContent.Create(order);
            return await ForwardContent(content);
        }

        private async Task<IActionResult> ForwardContent(HttpContent content)
        {
            var msg = new HttpRequestMessage(
                HttpMethod.Parse(HttpContext.Request.Method),
                TargetMicroUrl + HttpContext.Request.Path
            )
            {
                Content = content,
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
            var responseHeaders = resp.Headers.NonValidated.ToDictionary();
            foreach (var header in responseHeaders)
            {
                if (HttpContext.Response.Headers.ContainsKey(header.Key))
                {
                    HttpContext.Response.Headers.Remove(header.Key);
                }

                HttpContext.Response.Headers.Append(header.Key, header.Value.FirstOrDefault()); 
            }
            HttpContext.Response.Headers.Remove("Transfer-Encoding");

            var responseBody = await resp.Content.ReadAsStringAsync();
            var json = JsonNode.Parse(responseBody);
            var result = new ObjectResult(json)
            {
                StatusCode = (int)resp.StatusCode,
            };
            return result; 
        }
    }
}
