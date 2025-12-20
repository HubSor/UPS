using Dtos;
using TestHelpers;
using Messages.Clients;
using Models.Entities;
using NUnit.Framework;
using Services.Application;

namespace UnitTests.Clients;

[TestFixture]
public class ListCompanyClientsConsumerTests : ServiceTestCase<ClientsApplicationService, ListCompanyClientsOrder, ListCompanyClientsResponse>
{
	private PaginationDto Pagination = default!;
	private MockRepository<CompanyClient> companyClients = default!;
	
	protected override Task SetUp()
	{
		Pagination = new();
		
		companyClients = new MockRepository<CompanyClient>();
		companyClients.Entities.AddRange(new List<CompanyClient>()
		{
			new ()
			{
				Id = 1,
				PhoneNumber = "123456789",
				Regon = "123456789",
				CompanyName = "krzak"
			},
			new ()
			{
				Id = 2,
				Email = "januszex@gmail.com",
				Nip = "1234567890",
				CompanyName = "januszex"
			},
		});

		var personClients = new MockRepository<PersonClient>();
		var clients = new MockRepository<Client>();
		
		service = new ClientsApplicationService(mockLogger.Object, mockUnitOfWork.Object, clients.Object, personClients.Object, companyClients.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_OnePage()
	{
		var order = new ListCompanyClientsOrder(Pagination);
		
		var resp = await service.ListCompaniesAsync(order);
		var data = resp?.Clients;
		
		Assert.That(data, Is.Not.Null);
		Assert.That(data!.Pagination.Count, Is.EqualTo(2));
		Assert.That(data!.Pagination.TotalCount, Is.EqualTo(2));
		Assert.That(data!.Pagination.TotalPages, Is.EqualTo(1));
		Assert.That(data!.Pagination.PageSize, Is.EqualTo(10));
		Assert.That(data!.Pagination.PageIndex, Is.EqualTo(0));
		
		Assert.That(data!.Items.First().Id > data!.Items.Last().Id);
		Assert.That(data!.Items.Count(u => u.CompanyName == "januszex"), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.CompanyName == "krzak"), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.Regon == "123456789"), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.PhoneNumber == "123456789"), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.Nip == "1234567890"), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.Email == "januszex@gmail.com"), Is.EqualTo(1));
	}
}
