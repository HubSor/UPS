using Consumers.Products;
using Helpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Products;

[TestFixture]
public class AddSubProductConsumerTests : ConsumerTestCase<AddSubProductConsumer, AddSubProductOrder, AddSubProductResponse>
{
	private MockRepository<SubProduct> products = default!;

	protected override Task SetUp()
	{
		products = new MockRepository<SubProduct>();

		consumer = new AddSubProductConsumer(mockLogger.Object, products.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_AddSimpleSubProduct()
	{
		var order = new AddSubProductOrder( "TEST1", "Nowy podprodukt", 99.99m, "opis");
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newProduct = products.Entities.SingleOrDefault();
		Assert.That(newProduct, Is.Not.Null);
	
		Assert.That(newProduct!.Code, Is.EqualTo("TEST1"));
		Assert.That(newProduct!.Name, Is.EqualTo("Nowy podprodukt"));
		Assert.That(newProduct!.BasePrice, Is.EqualTo(99.99m));
		Assert.That(newProduct!.Description, Is.EqualTo("opis"));
	}
	
	[Test]
	public async Task Consume_BadRequest_CodeTaken()
	{
		products.Entities.Add(new() 
		{
			Name = "drugi podprodukt",
			Code = "TEST1"
		});
		
		var order = new AddSubProductOrder("test1", "Nowy podprodukt", 99.99m, null);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}
