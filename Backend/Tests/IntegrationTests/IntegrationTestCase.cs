using System.Text;
using System.Text.Json;
using Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;


namespace IntegrationTests;

public abstract class IntegrationTestCase : IClassFixture<UPSWebApplicationFactory<Program>>
{
	protected readonly WebApplicationFactory<Program> factory;
	protected readonly HttpClient client;
	protected string AuthCookie { get; set; } = "";
	
	public IntegrationTestCase(UPSWebApplicationFactory<Program> factory)
	{
		this.factory = factory;
		client = factory.CreateClient();

		OneTimeSetUp();
	} 
	
	protected HttpRequestMessage GetRequestMessage(string url, object request)
	{
		var msg = new HttpRequestMessage(HttpMethod.Post, url)
		{
			Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"),
		};
		
		if (!string.IsNullOrEmpty(AuthCookie))
			msg.Headers.Add("Cookie", AuthCookie);
		
		return msg;
	}
	
	protected async Task<ApiResponse<T>?> GetApiResponseAsync<T>(HttpResponseMessage httpResponse) where T : class
	{
		var json = await httpResponse.Content.ReadAsStringAsync();
		CheckAuthCookie(httpResponse);

		return JsonSerializer.Deserialize<ApiResponse<T>>(json,
			new JsonSerializerOptions() { 
				PropertyNameCaseInsensitive = true,
				IncludeFields = true,
			}
		);
	}
	
	protected void CheckAuthCookie(HttpResponseMessage httpResponse)
	{
		var authCookie = httpResponse.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value;
		if (authCookie != null && authCookie.Any() && authCookie.First().StartsWith("UPSAuth="))
			AuthCookie = authCookie.First();
	}

	protected virtual void OneTimeSetUp() {}
}
