using Consumers.Products;
using TestHelpers;
using MassTransit.Mediator;
using Messages.Products;
using Models.Entities;
using Moq;
using NUnit.Framework;

namespace UnitTests.Products;

[TestFixture]
public class AddSubProductConsumerTests : ConsumerTestCase<AddSubProductConsumer, AddSubProductOrder, AddSubProductResponse>
{
	private MockRepository<SubProduct> subProducts = default!;

	protected override Task SetUp()
	{
		subProducts = new MockRepository<SubProduct>();
		
		var mediator = new Mock<IMediator>();

		consumer = new AddSubProductConsumer(mockLogger.Object, subProducts.Object, mockUnitOfWork.Object, mediator.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_AddSimpleSubProduct()
	{
		var order = new AddSubProductOrder( "TEST1", "Nowy podprodukt", 99.99m, "opis", 0, null);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newProduct = subProducts.Entities.SingleOrDefault();
		Assert.That(newProduct, Is.Not.Null);
	
		Assert.That(newProduct!.Code, Is.EqualTo("TEST1"));
		Assert.That(newProduct!.Name, Is.EqualTo("Nowy podprodukt"));
		Assert.That(newProduct!.BasePrice, Is.EqualTo(99.99m));
		Assert.That(newProduct!.Description, Is.EqualTo("opis"));
		Assert.That(newProduct!.TaxRate, Is.EqualTo(0.00m));
	}

	[Test]
	public async Task Consume_BadRequest_AddSubProductNoRequestClient()
	{
		var order = new AddSubProductOrder( "TEST1", "Nowy podprodukt", 99.99m, "opis", 10, 1);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
	
	[Test]
	public async Task Consume_BadRequest_CodeTaken()
	{
		subProducts.Entities.Add(new() 
		{
			Name = "drugi podprodukt",
			Code = "TEST1"
		});
		
		var order = new AddSubProductOrder("test1", "Nowy podprodukt", 99.99m, null, 10, null);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}
