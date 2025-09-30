using TestHelpers;
using NUnit.Framework;

namespace UnitTests.Products;

[TestFixture]
public class EditSubProductAssignmentConsumerTests : ConsumerTestCase<EditSubProductAssignmentConsumer, EditSubProductAssignmentOrder, EditSubProductAssignmentResponse>
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

		consumer = new EditSubProductAssignmentConsumer(mockLogger.Object, intersection.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_Edited()
	{
		var order = new EditSubProductAssignmentOrder(1, 1, 0.75m);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var edited = intersection.Entities.SingleOrDefault();
		Assert.That(edited, Is.Not.Null);
	
		Assert.That(edited!.InProductPrice, Is.EqualTo(order.NewPrice));
	}
	
	[Test]
	public async Task Consume_BadRequest_NotFound()
	{
		var order = new EditSubProductAssignmentOrder(10, 10, 0.75m);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}