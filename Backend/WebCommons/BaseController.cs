using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebCommons
{
	[ApiController]
	public abstract class BaseController : ControllerBase
	{
		protected abstract string TargetMicroUrl { get; }

		protected HttpClient _httpClient = new();

		protected async Task<IActionResult> RelayMessage()
		{
			var original = HttpContext.Request;

			original.Headers.Remove("host");

			var msg = new HttpRequestMessage(
				HttpMethod.Parse(original.Method),
				TargetMicroUrl + "/" + original.Path
			)
			{
				Content = new StreamContent(original.Body),
			};

			var resp = await _httpClient.SendAsync(msg);

			return new ObjectResult(resp);
		}
	}
}
