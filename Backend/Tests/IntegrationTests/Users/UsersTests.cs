using System.Net;
using Core.Dtos;
using Core.Messages;
using Xunit;

namespace IntegrationTests.Users;

public class UsersTests(UPSWebApplicationFactory<Program> factory) : IntegrationTestCase(factory)
{
    [Fact]
	public async Task Login_Ok_CorrectCredentialsAdmin()
	{
		var req = GetRequestMessage("/users/login", new LoginOrder("admin", "admin"));
		
		var response = await client.SendAsync(req);
		response.EnsureSuccessStatusCode();
		var apiResponse = await GetApiResponseAsync<LoginResponse>(response);
		
		Assert.NotNull(apiResponse);
		Assert.True(apiResponse.Success);
		var userDto = apiResponse.Data!.UserDto;
		
		Assert.Equal("admin", userDto.Username);
		Assert.Equal(1, userDto.Id);
	}

	[Fact]
	public async Task Logout_BadRequest_Unauthorized()
	{
		AuthCookie = "";
		var req = GetRequestMessage("/users/logout", new LogoutOrder());
		var response = await client.SendAsync(req);
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task List_BadRequest_NoPermissionsAsUser()
	{
		var req = GetRequestMessage("/users/login", new LoginOrder("test", "admin"));
		var response = await client.SendAsync(req);
		response.EnsureSuccessStatusCode();
		CheckAuthCookie(response);

		var req2 = GetRequestMessage("/users/list", new ListUsersOrder(new PaginationDto 
		{
			PageIndex = 0, PageSize = 4
		}));
		response = await client.SendAsync(req2);
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task List_Ok_AsAdmin()
	{
		var req = GetRequestMessage("/users/login", new LoginOrder("admin", "admin"));
		var response = await client.SendAsync(req);
		response.EnsureSuccessStatusCode();
		CheckAuthCookie(response);

		var req2 = GetRequestMessage("/users/list", new ListUsersOrder(new PaginationDto
		{
			PageIndex = 0,
			PageSize = 4
		}));
		response = await client.SendAsync(req2);
		response.EnsureSuccessStatusCode();
		
		var apiResponse = await GetApiResponseAsync<ListUsersResponse>(response);
		Assert.NotNull(apiResponse);
		Assert.True(apiResponse.Success);
		
		Assert.Equal(2, apiResponse.Data!.Users.Pagination.TotalCount);
		Assert.Equal(4, apiResponse.Data!.Users.Pagination.PageSize);
		Assert.Contains(apiResponse.Data!.Users.Items, u => u.Id == 1 && u.Username == "admin");
		Assert.Contains(apiResponse.Data!.Users.Items, u => u.Id == 2 && u.Username == "test");
	}

	[Fact]
	public async Task Logout_Ok_AfterLogin()
	{
		var req = GetRequestMessage("/users/login", new LoginOrder("admin", "admin"));
		var response = await client.SendAsync(req);
		response.EnsureSuccessStatusCode();
		CheckAuthCookie(response);

		var req2 = GetRequestMessage("/users/logout", new LogoutOrder());
		response = await client.SendAsync(req2);
		response.EnsureSuccessStatusCode();
		
		var apiResponse = GetApiResponse(response);

		Assert.NotNull(apiResponse);
		Assert.True(apiResponse.StatusCode == HttpStatusCode.OK);
	}

	[Fact]
	public async Task Login_Ok_CorrectCredentialsUser()
	{
		var req = GetRequestMessage("/users/login", new LoginOrder("test", "admin"));

		var response = await client.SendAsync(req);

		response.EnsureSuccessStatusCode();
		var apiResponse = await GetApiResponseAsync<LoginResponse>(response);

		Assert.NotNull(apiResponse);
		Assert.True(apiResponse.Success);
		var userDto = apiResponse.Data!.UserDto;

		Assert.Equal("test", userDto.Username);
		Assert.Equal(2, userDto.Id);
	}

	[Fact]
	public async Task Session_Ok_AfterLoginUser()
	{
		var req = GetRequestMessage("/users/login", new LoginOrder("test", "admin"));
		var response = await client.SendAsync(req);
		response.EnsureSuccessStatusCode();
		CheckAuthCookie(response);

		var req2 = GetRequestMessage("/users/logout", new SessionOrder());
		response = await client.SendAsync(req2);
		response.EnsureSuccessStatusCode();
	}
}
