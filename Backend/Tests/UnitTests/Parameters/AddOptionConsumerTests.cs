using Consumers.Command;
using Helpers;
using Messages.Commands;
using Messages.Responses;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Parameters;

[TestFixture]
public class AddOptionConsumerTests : ConsumerTestCase<AddOptionConsumer, AddOptionOrder, AddOptionResponse>
{
	private MockRepository<Parameter> parameters = default!;
	private MockRepository<ParameterOption> options = default!;

	protected override Task SetUp()
	{
		parameters = new MockRepository<Parameter>();
		parameters.Entities.Add(new()
		{
			Id = 1,
			Type = ParameterTypeEnum.Text
		});
		parameters.Entities.Add(new()
		{
			Id = 2,
			Type = ParameterTypeEnum.Select
		});
		options = new MockRepository<ParameterOption>();

		consumer = new AddOptionConsumer(mockLogger.Object, parameters.Object, options.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_Add()
	{
		var order = new AddOptionOrder(2, "test535");
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newOption = options.Entities.SingleOrDefault();
		Assert.That(newOption, Is.Not.Null);
	
		Assert.That(newOption!.Value, Is.EqualTo("test535"));
		Assert.That(newOption!.ParameterId, Is.EqualTo(2));
	}

	[Test]
	public async Task Consume_BadRequest_AddToWrongType()
	{
		var order = new AddOptionOrder(1, "test535");

		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();

		var newOption = options.Entities.SingleOrDefault();
		Assert.That(newOption, Is.Null);
	}
}