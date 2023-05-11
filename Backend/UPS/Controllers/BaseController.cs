using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace UPS.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IMediator Mediator { get; set; }
        public BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}
