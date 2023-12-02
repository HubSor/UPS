using Consumers.Products;
using Helpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Products;

[TestFixture]
public class EditSubProductConsumerTests : ConsumerTestCase<EditSubProductConsumer, EditSubProductOrder, EditSubProductResponse>
{
	private MockRepository<SubProduct> subProducts = default!;

	protected override Task SetUp()
	{
		subProducts = new MockRepository<SubProduct>();
		subProducts.Entities.Add(new()
		{
			Id = 1,
			Name = "test",
			BasePrice = 99m,
			Code = "TEST1"
		});

		consumer = new EditSubProductConsumer(mockLogger.Object, subProducts.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_Edit()
	{
		var order = new EditSubProductOrder("CODE", "new", 10.99m, 10, "test", 1);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var edited = subProducts.Entities.SingleOrDefault();
		Assert.That(edited, Is.Not.Null);
	
		Assert.That(edited!.Code, Is.EqualTo("CODE"));
		Assert.That(edited!.Name, Is.EqualTo("new"));
		Assert.That(edited!.BasePrice, Is.EqualTo(10.99m));
		Assert.That(edited!.Description, Is.EqualTo("test"));
	}
}