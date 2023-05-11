using MassTransit.Mediator;
using Messages.Products;
using Microsoft.AspNetCore.Mvc;

namespace UPS.Controllers
{
    [Route("test")]
    public class TestController : BaseController
    {
        public TestController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("get")]
        public IActionResult Get()
        {
            Console.WriteLine("test");
            return Ok();
        }

        [HttpPost]
        [Route("addproduct")]
        public async Task<AddProductResponse> AddProduct([FromBody] AddProductOrder order)
        {
            var client = Mediator.CreateRequestClient<AddProductOrder>();
            var response = await client.GetResponse<AddProductResponse>(order);
            return response.Message;
        }

        [HttpGet]
        [Route("viewproducts")]
        public async Task<ViewProductsResponse> ViewProducts()
        {
            var client = Mediator.CreateRequestClient<ViewProductsOrder>();
            var response = await client.GetResponse<ViewProductsResponse>(new ViewProductsOrder());
            return response.Message;
        }
    }
}
