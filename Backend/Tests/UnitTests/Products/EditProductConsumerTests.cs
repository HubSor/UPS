using TestHelpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;
using Services.Application;

namespace UnitTests.Products;

[TestFixture]
public class EditProductConsumerTests : ServiceTestCase<ProductsApplicationService, EditProductOrder, EditProductResponse>
{
	private MockRepository<Product> products = default!;

	protected override Task SetUp()
	{
		products = new MockRepository<Product>();
		products.Entities.Add(new()
		{
			Id = 1,
			Name = "test",
			BasePrice = 99m,
			Status = ProductStatusEnum.NotOffered,
			AnonymousSaleAllowed = true,
			Code = "TEST1",
			TaxRate = 0.23m
		});

		service = new ProductsApplicationService(mockLogger.Object, mockUnitOfWork.Object, products.Object, GetMockRepo<SubProduct>(), GetMockRepo<SubProductInProduct>(), GetMockRepo<Parameter>());
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_Edit()
	{
		var order = new EditProductOrder(false, "CODE", "new", 10.99m, 10, "test", 1, ProductStatusEnum.Withdrawn);
		
		await service.EditProductAsync(order);
		
		var edited = products.Entities.SingleOrDefault();
		Assert.That(edited, Is.Not.Null);
	
		Assert.That(edited!.Code, Is.EqualTo("CODE"));
		Assert.That(edited!.Name, Is.EqualTo("new"));
		Assert.That(edited!.BasePrice, Is.EqualTo(10.99m));
		Assert.That(edited!.Description, Is.EqualTo("test"));
		Assert.That(edited!.Status, Is.EqualTo(ProductStatusEnum.Withdrawn));
		Assert.That(edited!.TaxRate, Is.EqualTo(0.1m));
	}
}