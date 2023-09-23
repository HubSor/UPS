using MassTransit.Mediator;
using Messages.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UPS.Controllers
{
    [Route("user")]
	public class UserController : BaseController
	{
		public UserController(IMediator mediator)
			: base(mediator)
		{
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginOrder order)
		{
			return await RespondAsync<LoginOrder, LoginResponse>(order);
		}
		
		[HttpPost]
		[Authorize]
		[Route("logout")]
		public async Task<IActionResult> Logout([FromBody] LogoutOrder order)
		{
			return await RespondAsync<LogoutOrder, LogoutResponse>(order);
		}
	}
}
