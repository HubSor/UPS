using Microsoft.AspNetCore.Mvc;
using MassTransit.Mediator;
using Models.Entities;
using UPS.Attributes;
using Messages.Products;

namespace UPS.Controllers
{
	[Route(template: "products")]
	public class ProductsController : BaseController
	{
		public ProductsController(IMediator mediator)
			: base(mediator)
		{
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> Add([FromBody] AddProductOrder order)
		{
			return await RespondAsync<AddProductOrder, AddProductResponse>(order);
		}
	}
}
