using Messages.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models.Entities;
using WebCommons;

namespace UPS.Controllers
{
	[Route("users")]
	public class UsersController : BaseController
	{
		protected override string TargetMicroUrl => "";

        [HttpPost]
		[Authorize]
		[Route("session")]
		public async Task<IActionResult> SessionAsync()
		{
			return await RelayMessage();
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginOrder order)
		{
			return await RelayMessage();
		}
		
		[HttpPost]
		[Authorize]
		[Route("logout")]
		public async Task<IActionResult> Logout([FromBody] LogoutOrder order)
		{
			return await RelayMessage();
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> AddUser([FromBody] AddUserOrder order)
		{
			return await RelayMessage();
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("list")]
		public async Task<IActionResult> ListUsers([FromBody] ListUsersOrder order)
		{
			return await RelayMessage();
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditUserOrder order)
		{
			return await RelayMessage();
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.UserManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteUserOrder order)
		{
			return await RelayMessage();
		}
	}
}
