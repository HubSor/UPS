using Consumers.Products;
using Helpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Products;

[TestFixture]
public class AddProductConsumerTests : ConsumerTestCase<AddProductConsumer, AddProductOrder, AddProductResponse>
{
	private MockRepository<Product> products = default!;

	protected override Task SetUp()
	{
		products = new MockRepository<Product>();

		consumer = new AddProductConsumer(mockLogger.Object, products.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_AddSimpleProduct()
	{
		var order = new AddProductOrder(true, "TEST1", "Nowy produkt", 99.99m, 10, null);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newProduct = products.Entities.SingleOrDefault();
		Assert.That(newProduct, Is.Not.Null);
	
		Assert.That(newProduct!.Code, Is.EqualTo("TEST1"));
		Assert.That(newProduct!.Name, Is.EqualTo("Nowy produkt"));
		Assert.That(newProduct!.BasePrice, Is.EqualTo(99.99m));
		Assert.That(newProduct!.Description, Is.Null);
		Assert.That(newProduct!.Status, Is.EqualTo(ProductStatusEnum.NotOffered));
	}
	
	[Test]
	public async Task Consume_BadRequest_CodeTaken()
	{
		products.Entities.Add(new() 
		{
			Name = "drugi produkt",
			Code = "TEST1"
		});
		
		var order = new AddProductOrder(true, "test1", "Nowy produkt", 99.99m, 10, null);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}
