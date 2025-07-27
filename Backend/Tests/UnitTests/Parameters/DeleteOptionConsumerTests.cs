using Consumers.Parameters;
using TestHelpers;
using Messages.Parameters;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Parameters;

[TestFixture]
public class DeleteOptionConsumerTests : ConsumerTestCase<DeleteOptionConsumer, DeleteOptionOrder, DeleteOptionResponse>
{
	private MockRepository<ParameterOption> options = default!;

	protected override Task SetUp()
	{
		options = new MockRepository<ParameterOption>();
		options.Entities.Add(new ParameterOption()
		{
			Id = 1,
			Value = "test"
		});

		consumer = new DeleteOptionConsumer(mockLogger.Object, options.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_Delete()
	{
		var order = new DeleteOptionOrder(1);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		Assert.That(options.Entities.Count, Is.EqualTo(0));
	}
}