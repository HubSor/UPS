using Microsoft.AspNetCore.Mvc;
using MassTransit.Mediator;
using Models.Entities;
using UPS.Attributes;
using Messages.Products;
using Microsoft.AspNetCore.Authorization;
using Services.Application;

namespace UPS.Controllers
{
	[Route(template: "products")]
	public class ProductsController(IServiceProvider sp, ProductsApplicationService productsApplicationService) : BaseController(sp)
	{
        [HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator, RoleEnum.Seller)]
		[Route("get")]
		public async Task<IActionResult> Get([FromBody] GetProductOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.GetProductAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("add")]
		public async Task<IActionResult> Add([FromBody] AddProductOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.AddProductAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("edit")]
		public async Task<IActionResult> Edit([FromBody] EditProductOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.EditProductAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("delete")]
		public async Task<IActionResult> Delete([FromBody] DeleteProductOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.DeleteProductAsync(order));
		}

		[HttpPost]
		[Authorize]
		[Route("list")]
		public async Task<IActionResult> List([FromBody] ListProductsOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.ListProductAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/get")]
		public async Task<IActionResult> GetSubProduct([FromBody] GetSubProductOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.GetSubProductAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/add")]
		public async Task<IActionResult> AddSubProduct([FromBody] AddSubProductOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.AddSubProductAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/edit")]
		public async Task<IActionResult> EditSubProduct([FromBody] EditSubProductOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.EditSubProductAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/delete")]
		public async Task<IActionResult> DeleteSubProduct([FromBody] DeleteSubProductOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.DeleteSubProductAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/assign")]
		public async Task<IActionResult> AssignSubProduct([FromBody] AssignSubProductOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.AssignSubProductAsync(order));
		}
		
		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/unassign")]
		public async Task<IActionResult> UnassignSubProducts([FromBody] UnassignSubProductsOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.UnassignSubProductsAsync(order));
		}
		
		[HttpPost]
		[Authorize]
		[Route("subproducts/list")]
		public async Task<IActionResult> ListSubProducts([FromBody] ListSubProductsOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.ListSubProductsAsync(order));
		}

		[HttpPost]
		[AuthorizeRoles(RoleEnum.ProductManager, RoleEnum.Administrator)]
		[Route("subproducts/assignments/edit")]
		public async Task<IActionResult> EditSubProductAssignment([FromBody] EditSubProductAssignmentOrder order)
		{
			return await PerformAction(order, productsApplicationService, () => productsApplicationService.EditSubProductAssignmentAsync(order));
		}
	}
}
