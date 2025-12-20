using TestHelpers;
using Messages.Products;
using Models.Entities;
using Moq;
using NUnit.Framework;
using Services.Application;
using FluentValidation;

namespace UnitTests.Products;

[TestFixture]
public class AddSubProductConsumerTests : ServiceTestCase<ProductsApplicationService, AddSubProductOrder, AddSubProductResponse>
{
	private MockRepository<SubProduct> subProducts = default!;

	protected override Task SetUp()
	{
		subProducts = new MockRepository<SubProduct>();
		
		service = new ProductsApplicationService(mockLogger.Object, mockUnitOfWork.Object, GetMockRepo<Product>(), subProducts.Object, GetMockRepo<SubProductInProduct>(), GetMockRepo<Parameter>());
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_AddSimpleSubProduct()
	{
		var order = new AddSubProductOrder( "TEST1", "Nowy podprodukt", 99.99m, "opis", 0, null);
		
		await service.AddSubProductAsync(order);
		
		var newProduct = subProducts.Entities.SingleOrDefault();
		Assert.That(newProduct, Is.Not.Null);
	
		Assert.That(newProduct!.Code, Is.EqualTo("TEST1"));
		Assert.That(newProduct!.Name, Is.EqualTo("Nowy podprodukt"));
		Assert.That(newProduct!.BasePrice, Is.EqualTo(99.99m));
		Assert.That(newProduct!.Description, Is.EqualTo("opis"));
		Assert.That(newProduct!.TaxRate, Is.EqualTo(0.00m));
	}

	[Test]
	public void Consume_BadRequest_AddSubProductNoRequestClient()
	{
		var order = new AddSubProductOrder( "TEST1", "Nowy podprodukt", 99.99m, "opis", 10, 1);
		
		Assert.ThrowsAsync<ValidationException>(() => service.AddSubProductAsync(order));
	}
	
	[Test]
	public void Consume_BadRequest_CodeTaken()
	{
		subProducts.Entities.Add(new() 
		{
			Name = "drugi podprodukt",
			Code = "TEST1"
		});
		
		var order = new AddSubProductOrder("test1", "Nowy podprodukt", 99.99m, null, 10, null);
		
		Assert.ThrowsAsync<ValidationException>(() => service.AddSubProductAsync(order));
	}
}
