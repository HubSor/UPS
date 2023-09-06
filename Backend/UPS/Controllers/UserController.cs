using MassTransit.Mediator;
using Messages.Users;
using Microsoft.AspNetCore.Mvc;
using Core;

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
        public async Task<ApiResponse<LoginResponse>> Login([FromBody] LoginOrder order)
        {
            return await RespondAsync<LoginOrder, LoginResponse>(order);
        }
    }
}
