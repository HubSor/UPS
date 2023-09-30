﻿using MassTransit.Mediator;
using Messages.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UPS.Controllers
{
	[Route("users")]
	public class UsersController : BaseController
	{
		public UsersController(IMediator mediator)
			: base(mediator)
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
		
		[HttpPost]
		[Authorize]
		[Route("add")]
		public async Task<IActionResult> AddUser([FromBody] AddUserOrder order)
		{
			return await RespondAsync<AddUserOrder, AddUserResponse>(order);
		}
	}
}
