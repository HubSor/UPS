using TestHelpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;
using Services.Application;
using FluentValidation;

namespace UnitTests.Products;

[TestFixture]
public class AddProductConsumerTests : ServiceTestCase<ProductsApplicationService, AddProductOrder, AddProductResponse>
{
	private MockRepository<Product> products = default!;

	protected override Task SetUp()
	{
		products = new MockRepository<Product>();

		service = new ProductsApplicationService(mockLogger.Object, mockUnitOfWork.Object, products.Object, GetMockRepo<SubProduct>(), GetMockRepo<SubProductInProduct>(), GetMockRepo<Parameter>());
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_AddSimpleProduct()
	{
		var order = new AddProductOrder(true, "TEST1", "Nowy produkt", 99.99m, 10, null);
		
		await service.AddProductAsync(order);
		
		var newProduct = products.Entities.SingleOrDefault();
		Assert.That(newProduct, Is.Not.Null);
	
		Assert.That(newProduct!.Code, Is.EqualTo("TEST1"));
		Assert.That(newProduct!.Name, Is.EqualTo("Nowy produkt"));
		Assert.That(newProduct!.BasePrice, Is.EqualTo(99.99m));
		Assert.That(newProduct!.Description, Is.Null);
		Assert.That(newProduct!.Status, Is.EqualTo(ProductStatusEnum.NotOffered));
		Assert.That(newProduct!.TaxRate, Is.EqualTo(0.1m));
	}
	
	[Test]
	public void Consume_BadRequest_CodeTaken()
	{
		products.Entities.Add(new() 
		{
			Name = "drugi produkt",
			Code = "TEST1"
		});
		
		var order = new AddProductOrder(true, "test1", "Nowy produkt", 99.99m, 10, null);
		
		Assert.ThrowsAsync<ValidationException>(() => service.AddProductAsync(order));
	}
}
