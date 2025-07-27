using TestHelpers;
using Models.Entities;
using NUnit.Framework;
using Consumers.Command;
using Messages.Commands;
using Messages.Responses;

namespace UnitTests.Clients;

[TestFixture]
public class UpsertClientConsumerTests : ConsumerTestCase<UpsertClientConsumer, UpsertClientOrder, UpsertClientResponse>
{
	private MockRepository<Client> clients = default!;

	protected override Task SetUp()
	{
		clients = new MockRepository<Client>();
		
		clients.Entities.Add(new CompanyClient() 
		{
			Id = 1,
			CompanyName = "krzak",
			PhoneNumber = "123456789",
			Email = null,
			Regon = "123456789",
			Nip = null,
		});

		consumer = new UpsertClientConsumer(mockLogger.Object, clients.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_AddPerson()
	{
		var order = new UpsertClientOrder(false, null, "123456789", "jan.los@gmail.com",
			"jan", "łoś", "12345678900", null, null, null);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newClient = clients.Entities.SingleOrDefault(x => x.Email == order.Email);
		Assert.That(newClient, Is.Not.Null);
		var personClient = newClient as PersonClient;
		Assert.That(personClient, Is.Not.Null);

		Assert.That(personClient!.LastName, Is.EqualTo(order.LastName));
		Assert.That(personClient!.Pesel, Is.EqualTo(order.Pesel));
		Assert.That(personClient!.FirstName, Is.EqualTo(order.FirstName));
	}

	[Test]
	public async Task Consume_Ok_UpdateCompany()
	{
		var order = new UpsertClientOrder(true, 1, "987654321", null,
			null, null, null, "drugi krzak", "000000000", null);

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		var newClient = clients.Entities.SingleOrDefault(x => x.Id == order.ClientId);
		Assert.That(newClient, Is.Not.Null);
		var company = newClient as CompanyClient;
		Assert.That(company, Is.Not.Null);

		Assert.That(company!.Regon, Is.EqualTo(order.Regon));
		Assert.That(company!.Email, Is.Null);
		Assert.That(company!.Nip, Is.Null);
		Assert.That(company!.CompanyName, Is.EqualTo(order.CompanyName));
	}

	[Test]
	public async Task Consume_BadRequest_ChangeClientType()
	{
		var order = new UpsertClientOrder(false, 1, "987654321", null,
			null, null, null, "drugi krzak", "000000000", null);

		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}
