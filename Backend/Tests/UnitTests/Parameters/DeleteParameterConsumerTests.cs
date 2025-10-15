using TestHelpers;
using NUnit.Framework;
using Core.Models;
using ProductsMicro.Consumers;
using Core.Messages;

namespace UnitTests.Parameters;

[TestFixture]
public class DeleteParameterConsumerTests : ConsumerTestCase<DeleteParameterConsumer, DeleteParameterOrder, DeleteParameterResponse>
{
	private MockRepository<Parameter> parameters = default!;

	protected override Task SetUp()
	{
		parameters = new MockRepository<Parameter>();
		parameters.Entities.Add(new Parameter()
		{
			Id = 1,
			Name = "test",
			Required = true,
			Type = ParameterTypeEnum.Select,
			SaleParameters = new List<SaleParameter>()
			{
				new() 
				{
					ParameterId = 1
				}
			}
		});

		parameters.Entities.Add(new Parameter()
		{
			Id = 2,
			Name = "test2",
			Required = false,
			Type = ParameterTypeEnum.Text,
			SaleParameters = new List<SaleParameter>() {}
		});

		consumer = new DeleteParameterConsumer(mockLogger.Object, parameters.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_DeleteHard()
	{
		var order = new DeleteParameterOrder(2);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		Assert.That(parameters.Entities.Count, Is.EqualTo(1));
		Assert.That(parameters.Entities.First().Id, Is.Not.EqualTo(2));
	}

	[Test]
	public async Task Consume_Ok_DeleteSoft()
	{
		var order = new DeleteParameterOrder(1);

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		Assert.That(parameters.Entities.Count, Is.EqualTo(2));
		var sofDeleted = parameters.Entities.Single(x => x.Id == 1);
		Assert.That(sofDeleted.Deleted, Is.EqualTo(true));
		Assert.That(sofDeleted.Name, Is.EqualTo("test"));
		Assert.That(sofDeleted.SaleParameters.Count, Is.EqualTo(1));
	}
}