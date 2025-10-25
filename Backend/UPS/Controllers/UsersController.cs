using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.Web;
using Core.Messages;
using Core.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace UPS.Controllers
{
	[Route("users")]
	public class UsersController(IServiceProvider sp) : BaseController(sp)
	{
        [HttpPost]
		[Authorize]
		[Route("session")]
		public IActionResult Session()
		{
			return Ok(HttpContext.User.Identity?.IsAuthenticated ?? false);
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginOrder order)
		{
			var resp = await GetApiResponse<LoginOrder, LoginResponse>(order);
			if (resp.Data?.Claims.Count > 0)
			{
				var claimsIdentity = new ClaimsIdentity(resp.Data.Claims.Select(x => x.ToClaim()), CookieAuthenticationDefaults.AuthenticationScheme);
				var principal = new ClaimsPrincipal(claimsIdentity);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

				resp.Data.Claims.Clear();
			}

			return new ObjectResult(resp)
            {
                StatusCode = (int)resp.StatusCode
            };
		}
		
		[HttpPost]
		[Authorize]
		[Route("logout")]
		public async Task<IActionResult> Logout([FromBody] LogoutOrder order)
		{
			await HttpContext.SignOutAsync();
			return new ObjectResult(new {})
            {
                StatusCode = 200,
            };
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> AddUser([FromBody] AddUserOrder order)
		{
			return await RespondAsync<AddUserOrder, AddUserResponse>(order);
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("list")]
		public async Task<IActionResult> ListUsers([FromBody] ListUsersOrder order)
		{
			return await RespondAsync<ListUsersOrder, ListUsersResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditUserOrder order)
		{
			var resp = await GetApiResponse<EditUserOrder, EditUserResponse>(order);
			if (resp.Data?.Claims.Count > 0)
			{
				var claimsIdentity = new ClaimsIdentity(resp.Data.Claims.Select(x => x.ToClaim()), CookieAuthenticationDefaults.AuthenticationScheme);
				var principal = new ClaimsPrincipal(claimsIdentity);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

				resp.Data.Claims.Clear();
			}

			return new ObjectResult(resp)
            {
                StatusCode = (int)resp.StatusCode
            };
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteUserOrder order)
		{
			return await RespondAsync<DeleteUserOrder, DeleteUserResponse>(order);
		}
	}
}
