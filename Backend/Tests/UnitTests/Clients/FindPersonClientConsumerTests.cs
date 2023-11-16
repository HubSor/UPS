using Consumers.Clients;
using Helpers;
using Messages.Clients;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Clients;

[TestFixture]
public class FindPersonClientConsumerTests : ConsumerTestCase<FindPersonClientConsumer, FindPersonClientOrder, FindPersonClientResponse>
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

		consumer = new FindPersonClientConsumer(mockLogger.Object, clients.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_FindByPesel()
	{
		var order = new FindPersonClientOrder(pesel);
		
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
		var order = new FindPersonClientOrder(pesel + '1');

		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}
