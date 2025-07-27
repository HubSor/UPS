using TestHelpers;
using Models.Entities;
using NUnit.Framework;
using Consumers.Query;
using Messages.Queries;
using Messages.Responses;

namespace UnitTests.Clients;

[TestFixture]
public class FindPersonClientConsumerTests : ConsumerTestCase<FindPersonClientConsumer, FindPersonClientQuery, FindPersonClientResponse>
{
	private MockRepository<PersonClient> clients = default!;
	private static readonly string pesel = "00123456789";
	protected override Task SetUp()
	{
		clients = new MockRepository<PersonClient>();

		clients.Entities.Add(new PersonClient()
		{
			Id = 1,
			FirstName = "jan",
			LastName = "łoś",
			PhoneNumber = "123456789",
			Email = null,
			Pesel = pesel
		});

		consumer = new FindPersonClientConsumer(mockLogger.Object, clients.Object);
		return Task.CompletedTask;
	}

	[Test]
	public async Task Consume_Ok_FindByPesel()
	{
		var order = new FindPersonClientQuery(pesel);

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		var personClient = responses.Single().Data?.PersonClient;
		Assert.That(personClient, Is.Not.Null);

		Assert.That(personClient!.FirstName, Is.EqualTo("jan"));
		Assert.That(personClient!.LastName, Is.EqualTo("łoś"));
		Assert.That(personClient!.Pesel, Is.EqualTo(pesel));
		Assert.That(personClient!.Email, Is.Null);
		Assert.That(personClient!.PhoneNumber, Is.EqualTo("123456789"));
	}

	[Test]
	public async Task Consume_BadRequest_NotFound()
	{
		var order = new FindPersonClientQuery(pesel + '1');

		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}
