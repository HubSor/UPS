using Messages.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UPS.Attributes;
using Models.Entities;
using Services.Application;

namespace UPS.Controllers
{
	[Route("users")]
	public class UsersController(
		IServiceProvider sp,
		IUsersApplicationService usersApplicationService
	) : BaseController(sp)
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
			return await PerformAction(order, usersApplicationService, () => usersApplicationService.LoginAsync(order));
		}
		
		[HttpPost]
		[Authorize]
		[Route("logout")]
		public async Task<IActionResult> Logout([FromBody] LogoutOrder order)
		{
			return await PerformAction(order, usersApplicationService, () => usersApplicationService.LogoutAsync(order));
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> AddUser([FromBody] AddUserOrder order)
		{
			return await PerformAction(order, usersApplicationService, () => usersApplicationService.AddUserAsync(order));
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("list")]
		public async Task<IActionResult> ListUsers([FromBody] ListUsersOrder order)
		{
			return await PerformAction(order, usersApplicationService, () => usersApplicationService.ListUsersAsync(order));
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditUserOrder order)
		{
			return await PerformAction(order, usersApplicationService, () => usersApplicationService.EditUserAsync(order));
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteUserOrder order)
		{
			return await PerformAction(order, usersApplicationService, () => usersApplicationService.DeleteUserAsync(order));
		}
	}
}
