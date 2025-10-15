using TestHelpers;
using NUnit.Framework;
using Core.Models;
using ProductsMicro.Consumers;
using Core.Messages;
using Core.Dtos;

namespace UnitTests.Parameters;

[TestFixture]
public class EditParameterConsumerTests : ConsumerTestCase<EditParameterConsumer, EditParameterOrder, EditParameterResponse>
{
	private MockRepository<Parameter> parameters = default!;
	private MockRepository<ParameterOption> options = default!;

	protected override Task SetUp()
	{
		var opts = new List<ParameterOption>()
		{
			new ()
			{
				Id = 1,
				Value = "1",
			},
			new ()
			{
				Id = 2,
				Value = "2",
			}
		};
		
		options = new MockRepository<ParameterOption>();
		options.Entities.AddRange(opts); 
		
		parameters = new MockRepository<Parameter>();
		parameters.Entities.Add(new Parameter()
		{
			Id = 1,
			Name = "test",
			Options = opts,
			Required = true,
			Type = ParameterTypeEnum.Select
		});

		consumer = new EditParameterConsumer(mockLogger.Object, parameters.Object, options.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_EditRemoveOptions()
	{
		var order = new EditParameterOrder(1, "new", true, ParameterTypeEnum.Text, null);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newParam = parameters.Entities.SingleOrDefault();
		Assert.That(newParam, Is.Not.Null);
	
		Assert.That(newParam!.Name, Is.EqualTo("new"));
		Assert.That(newParam!.Required, Is.EqualTo(true));
		Assert.That(newParam!.Type, Is.EqualTo(ParameterTypeEnum.Text));
		Assert.That(newParam!.Options, Is.Empty);
		
		Assert.That(options.Entities, Is.Empty);
	}

	[Test]
	public async Task Consume_Ok_EditReplaceOptions()
	{
		var order = new EditParameterOrder(1, "new2", false, ParameterTypeEnum.Select, new List<OptionDto>() 
		{
			new ()
			{
				Value = "new option"
			}
		});

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		var newParam = parameters.Entities.SingleOrDefault();
		Assert.That(newParam, Is.Not.Null);

		Assert.That(newParam!.Name, Is.EqualTo("new2"));
		Assert.That(newParam!.Required, Is.EqualTo(false));
		Assert.That(newParam!.Type, Is.EqualTo(ParameterTypeEnum.Select));
		Assert.That(newParam!.Options.Count, Is.EqualTo(1));
		Assert.That(newParam!.Options.Single().Value, Is.EqualTo("new option"));

		Assert.That(options.Entities.Count, Is.EqualTo(0));
	}

	[Test]
	public async Task Consume_Ok_NoChangeToOptions()
	{
		parameters.Entities.Single().Options.Clear();
		parameters.Entities.Single().Type = ParameterTypeEnum.Integer;
		
		var order = new EditParameterOrder(1, "new3", false, ParameterTypeEnum.Checkbox, null);

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		var newParam = parameters.Entities.SingleOrDefault();
		Assert.That(newParam, Is.Not.Null);

		Assert.That(newParam!.Name, Is.EqualTo("new3"));
		Assert.That(newParam!.Required, Is.EqualTo(false));
		Assert.That(newParam!.Type, Is.EqualTo(ParameterTypeEnum.Checkbox));
		Assert.That(newParam!.Options, Is.Empty);
	}
}
