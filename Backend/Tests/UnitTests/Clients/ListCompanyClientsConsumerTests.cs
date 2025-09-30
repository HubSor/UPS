using TestHelpers;
using NUnit.Framework;

namespace UnitTests.Clients;

[TestFixture]
public class ListCompanyClientsConsumerTests : ConsumerTestCase<ListCompanyClientsConsumer, ListCompanyClientsOrder, ListCompanyClientsResponse>
{
	private PaginationDto Pagination = default!;
	private MockRepository<CompanyClient> clients = default!;
	
	protected override Task SetUp()
	{
		Pagination = new();
		
		clients = new MockRepository<CompanyClient>();
		clients.Entities.AddRange(new List<CompanyClient>()
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
		
		consumer = new ListCompanyClientsConsumer(mockLogger.Object, clients.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_OnePage()
	{
		var order = new ListCompanyClientsOrder(Pagination);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var data = responses.Single().Data?.Clients;
		
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
