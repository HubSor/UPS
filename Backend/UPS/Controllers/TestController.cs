using Microsoft.AspNetCore.Mvc;

namespace UPS.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("get")]
        public IActionResult Get()
        {
            Console.WriteLine("test");
            return Ok();
        }
    }
}
