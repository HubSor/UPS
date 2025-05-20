using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using UPS.Attributes;
using Microsoft.AspNetCore.Authorization;
using Services.Application;
using Messages.Queries;
using Messages.Responses;
using Messages.Commands;

namespace UPS.Controllers
{
	[Route(template: "products")]
	public class ProductsController : BaseController
	{
		public ProductsController(ICqrsService cqrsService)
			: base(cqrsService)
		{
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator, RoleEnum.Seller)]
		[Route("get")]
		public async Task<IActionResult> Get([FromBody] GetProductQuery order)
		{
			return await PerformQuery<GetProductQuery, GetProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> Add([FromBody] AddProductOrder order)
		{
			return await PerformCommand<AddProductOrder, AddProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditProductOrder order)
		{
			return await PerformCommand<EditProductOrder, EditProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteProductOrder order)
		{
			return await PerformCommand<DeleteProductOrder, DeleteProductResponse>(order);
		}

		[HttpPost]
		[Authorize]
		[Route("list")]
		public async Task<IActionResult> List([FromBody] ListProductsQuery order)
		{
			return await PerformQuery<ListProductsQuery, ListProductsResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/get")]
		public async Task<IActionResult> GetSubProduct([FromBody] GetSubProductQuery order)
		{
			return await PerformQuery<GetSubProductQuery, GetSubProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/add")]
		public async Task<IActionResult> AddSubProduct([FromBody] AddSubProductOrder order)
		{
			return await PerformCommand<AddSubProductOrder, AddSubProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/edit")]
		public async Task<IActionResult> EditSubProduct([FromBody] EditSubProductOrder order)
		{
			return await PerformCommand<EditSubProductOrder, EditSubProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/delete")]
		public async Task<IActionResult> DeleteSubProduct([FromBody] DeleteSubProductOrder order)
		{
			return await PerformCommand<DeleteSubProductOrder, DeleteSubProductResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/assign")]
		public async Task<IActionResult> AssignSubProduct([FromBody] AssignSubProductOrder order)
		{
			return await PerformCommand<AssignSubProductOrder, AssignSubProductResponse>(order);
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/unassign")]
		public async Task<IActionResult> UnassignSubProducts([FromBody] UnassignSubProductsOrder order)
		{
			return await PerformCommand<UnassignSubProductsOrder, UnassignSubProductsResponse>(order);
		}
		
		[HttpPost]
		[Authorize]
		[Route("subproducts/list")]
		public async Task<IActionResult> ListSubProducts([FromBody] ListSubProductsQuery order)
		{
			return await PerformQuery<ListSubProductsQuery, ListSubProductsResponse>(order);
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/assignments/edit")]
		public async Task<IActionResult> EditSubProductAssignment([FromBody] EditSubProductAssignmentOrder order)
		{
			return await PerformCommand<EditSubProductAssignmentOrder, EditSubProductAssignmentResponse>(order);
		}
	}
}
