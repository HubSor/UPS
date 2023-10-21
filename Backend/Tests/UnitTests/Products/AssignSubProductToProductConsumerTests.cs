using Consumers.Products;
using Helpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Products;

[TestFixture]
public class AssignSubProductToProductConsumerTests : ConsumerTestCase<AssignSubProductToProductConsumer, AssignSubProductToProductOrder, AssignSubProductToProductResponse>
{
	private MockRepository<Product> products = default!;
	private MockRepository<SubProduct> subProducts = default!;
	private MockRepository<SubProductInProduct> intersection = default!;

	protected override Task SetUp()
	{
		subProducts = new MockRepository<SubProduct>();
		subProducts.Entities.Add(new()
		{
			Id = 1,
		});
		
		products = new MockRepository<Product>();
		products.Entities.Add(new()
		{
			Id = 1
		});
		
		intersection = new MockRepository<SubProductInProduct>();

		consumer = new AssignSubProductToProductConsumer(mockLogger.Object, subProducts.Object, 
			products.Object, intersection.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_AssignToProduct()
	{
		var order = new AssignSubProductToProductOrder(1, 1, 0.75m);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newIntersection = intersection.Entities.SingleOrDefault();
		Assert.That(newIntersection, Is.Not.Null);
	
		Assert.That(newIntersection!.InProductPrice, Is.EqualTo(order.Price));
	}
	
	[Test]
	public async Task Consume_BadRequest_AlreadyAssigned()
	{
		intersection.Entities.Add(new()
		{
			ProductId = 1,
			SubProductId = 1,
			InProductPrice = 0.99m
		});
		var order = new AssignSubProductToProductOrder(1, 1, 0.75m);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}