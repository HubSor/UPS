using TestHelpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;
using Services.Application;
using FluentValidation;

namespace UnitTests.Products;

[TestFixture]
public class EditSubProductAssignmentConsumerTests : ServiceTestCase<ProductsApplicationService, EditSubProductAssignmentOrder, EditSubProductAssignmentResponse>
{
	private MockRepository<SubProductInProduct> intersection = default!;

	protected override Task SetUp()
	{		
		intersection = new MockRepository<SubProductInProduct>();
		intersection.Entities.Add(new()
		{
			ProductId = 1,
			SubProductId = 1,
			InProductPrice = 10m
		});

		service = new ProductsApplicationService(mockLogger.Object, mockUnitOfWork.Object, GetMockRepo<Product>(), GetMockRepo<SubProduct>(), intersection.Object, GetMockRepo<Parameter>());
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_Edited()
	{
		var order = new EditSubProductAssignmentOrder(1, 1, 0.75m);
		
		await service.EditSubProductAssignmentAsync(order);
		
		var edited = intersection.Entities.SingleOrDefault();
		Assert.That(edited, Is.Not.Null);
	
		Assert.That(edited!.InProductPrice, Is.EqualTo(order.NewPrice));
	}
	
	[Test]
	public void Consume_BadRequest_NotFound()
	{
		var order = new EditSubProductAssignmentOrder(10, 10, 0.75m);
		
		Assert.ThrowsAsync<ValidationException>(() => service.EditSubProductAssignmentAsync(order));
	}
}