using Consumers.Products;
using Dtos.Products;
using Helpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Products;

[TestFixture]
public class EditProductConsumerTests : ConsumerTestCase<EditProductConsumer, EditProductOrder, EditProductResponse>
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
			Code = "TEST1"
		});

		consumer = new EditProductConsumer(mockLogger.Object, products.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_Edit()
	{
		var order = new EditProductOrder(false, "CODE", "new", 10.99m, "test", 1, ProductStatusEnum.Withdrawn);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var edited = products.Entities.SingleOrDefault();
		Assert.That(edited, Is.Not.Null);
	
		Assert.That(edited!.Code, Is.EqualTo("CODE"));
		Assert.That(edited!.Name, Is.EqualTo("new"));
		Assert.That(edited!.BasePrice, Is.EqualTo(10.99m));
		Assert.That(edited!.Description, Is.EqualTo("test"));
		Assert.That(edited!.Status, Is.EqualTo(ProductStatusEnum.Withdrawn));
	}
}