using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UPS.Attributes;
using Models.Entities;
using Services.Application;
using Messages.Queries;
using Messages.Responses;
using Messages.Commands;

namespace UPS.Controllers
{
	[Route("users")]
	public class UsersController : BaseController
	{
		public UsersController(ICqrsService cqrsService)
			: base(cqrsService)
		{
		}
		
		[HttpPost]
		[Authorize]
		[Route("session")]
		public IActionResult Session()
		{
			return Ok(HttpContext.User.Identity?.IsAuthenticated ?? false);
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginQuery order)
		{
			return await PerformQuery<LoginQuery, LoginResponse>(order);
		}
		
		[HttpPost]
		[Authorize]
		[Route("logout")]
		public async Task<IActionResult> Logout([FromBody] LogoutQuery order)
		{
			return await PerformQuery<LogoutQuery, LogoutResponse>(order);
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> AddUser([FromBody] AddUserOrder order)
		{
			return await PerformCommand<AddUserOrder, AddUserResponse>(order);
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("list")]
		public async Task<IActionResult> ListUsers([FromBody] ListUsersQuery order)
		{
			return await PerformQuery<ListUsersQuery, ListUsersResponse>(order);
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditUserOrder order)
		{
			return await PerformCommand<EditUserOrder, EditUserResponse>(order);
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteUserOrder order)
		{
			return await PerformCommand<DeleteUserOrder, DeleteUserResponse>(order);
		}
	}
}
