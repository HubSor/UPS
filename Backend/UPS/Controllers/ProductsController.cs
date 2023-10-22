using Microsoft.AspNetCore.Mvc;
using MassTransit.Mediator;
using Models.Entities;
using UPS.Attributes;
using Messages.Products;
using Microsoft.AspNetCore.Authorization;

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
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/add")]
		public async Task<IActionResult> AddSubProduct([FromBody] AddSubProductOrder order)
		{
			return await RespondAsync<AddSubProductOrder, AddSubProductResponse>(order);
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/assign")]
		public async Task<IActionResult> AssignSubProduct([FromBody] AssignSubProductOrder order)
		{
			return await RespondAsync<AssignSubProductOrder, AssignSubProductResponse>(order);
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/unassign")]
		public async Task<IActionResult> UnassignSubProducts([FromBody] UnassignSubProductsOrder order)
		{
			return await RespondAsync<UnassignSubProductsOrder, UnassignSubProductsResponse>(order);
		}
		
		[HttpPost]
		[Authorize]
		[Route("list")]
		public async Task<IActionResult> List([FromBody] ListProductsOrder order)
		{
			return await RespondAsync<ListProductsOrder, ListProductsResponse>(order);
		}
	}
}
