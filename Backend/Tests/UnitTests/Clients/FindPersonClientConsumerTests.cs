using TestHelpers;
using Messages.Clients;
using Models.Entities;
using NUnit.Framework;
using Services.Application;
using FluentValidation;

namespace UnitTests.Clients;

[TestFixture]
public class FindPersonClientConsumerTests : ServiceTestCase<ClientsApplicationService, FindPersonClientOrder, FindPersonClientResponse>
{
	private MockRepository<PersonClient> personClients = default!;
	private static readonly string pesel = "00123456789";
	protected override Task SetUp()
	{
		personClients = new MockRepository<PersonClient>();
		personClients.Entities.Add(new PersonClient()
		{
			Id = 1,
			FirstName = "jan",
			LastName = "łoś",
			PhoneNumber = "123456789",
			Email = null,
			Pesel = pesel
		});

		var companyClients = new MockRepository<CompanyClient>();
		var clients = new MockRepository<Client>();

		service = new ClientsApplicationService(mockLogger.Object, mockUnitOfWork.Object, clients.Object, personClients.Object, companyClients.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_FindByPesel()
	{
		var order = new FindPersonClientOrder(pesel);

		var resp = await service.FindPersonAsync(order);

		var personClient = resp.PersonClient;
		Assert.That(personClient, Is.Not.Null);

		Assert.That(personClient!.FirstName, Is.EqualTo("jan"));
		Assert.That(personClient!.LastName, Is.EqualTo("łoś"));
		Assert.That(personClient!.Pesel, Is.EqualTo(pesel));
		Assert.That(personClient!.Email, Is.Null);
		Assert.That(personClient!.PhoneNumber, Is.EqualTo("123456789"));
	}

	[Test]
	public void Consume_BadRequest_NotFound()
	{
		var order = new FindPersonClientOrder(pesel + '1');

		Assert.ThrowsAsync<ValidationException>(() => service.FindPersonAsync(order));
	}
}
