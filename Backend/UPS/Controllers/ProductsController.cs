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
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator, RoleEnum.Seller)]
		[Route("get")]
		public async Task<IActionResult> Get([FromBody] GetProductOrder order)
		{
			return await PerformQuery<GetProductOrder, GetProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> Add([FromBody] AddProductOrder order)
		{
			return await PerformQuery<AddProductOrder, AddProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditProductOrder order)
		{
			return await PerformQuery<EditProductOrder, EditProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteProductOrder order)
		{
			return await PerformQuery<DeleteProductOrder, DeleteProductResponse>(order);
		}

		[HttpPost]
		[Authorize]
		[Route("list")]
		public async Task<IActionResult> List([FromBody] ListProductsOrder order)
		{
			return await PerformQuery<ListProductsOrder, ListProductsResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/get")]
		public async Task<IActionResult> GetSubProduct([FromBody] GetSubProductOrder order)
		{
			return await PerformQuery<GetSubProductOrder, GetSubProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/add")]
		public async Task<IActionResult> AddSubProduct([FromBody] AddSubProductOrder order)
		{
			return await PerformQuery<AddSubProductOrder, AddSubProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/edit")]
		public async Task<IActionResult> EditSubProduct([FromBody] EditSubProductOrder order)
		{
			return await PerformQuery<EditSubProductOrder, EditSubProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/delete")]
		public async Task<IActionResult> DeleteSubProduct([FromBody] DeleteSubProductOrder order)
		{
			return await PerformQuery<DeleteSubProductOrder, DeleteSubProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/assign")]
		public async Task<IActionResult> AssignSubProduct([FromBody] AssignSubProductOrder order)
		{
			return await PerformQuery<AssignSubProductOrder, AssignSubProductResponse>(order);
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/unassign")]
		public async Task<IActionResult> UnassignSubProducts([FromBody] UnassignSubProductsOrder order)
		{
			return await PerformQuery<UnassignSubProductsOrder, UnassignSubProductsResponse>(order);
		}
		
		[HttpPost]
		[Authorize]
		[Route("subproducts/list")]
		public async Task<IActionResult> ListSubProducts([FromBody] ListSubProductsOrder order)
		{
			return await PerformQuery<ListSubProductsOrder, ListSubProductsResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/assignments/edit")]
		public async Task<IActionResult> EditSubProductAssignment([FromBody] EditSubProductAssignmentOrder order)
		{
			return await PerformQuery<EditSubProductAssignmentOrder, EditSubProductAssignmentResponse>(order);
		}
	}
}
