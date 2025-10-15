using TestHelpers;
using NUnit.Framework;
using SalesMicro.Consumers;
using Core.Messages;
using Core.Dtos;
using Core.Models;

namespace UnitTests.Sales;

[TestFixture]
public class ListSalesConsumerTests : ConsumerTestCase<ListSalesConsumer, ListSalesOrder, ListSalesResponse>
{
	private PaginationDto Pagination = default!;
	private MockRepository<Sale> sales = default!;
	
	protected override Task SetUp()
	{
		Pagination = new();
		
		sales = new MockRepository<Sale>();
		sales.Entities.AddRange(new List<Sale>()
		{
			new ()
			{
				Id = 1,
				SaleTime = new DateTime(2000, 1, 1),
				FinalPrice = 99.99m,
				ProductCode = "TEST01",
				SubProductCodes = "SUB01;SUB02",
			},
			new ()
			{
				Id = 2,
				SaleTime = new DateTime(2020, 1, 1, 15, 15, 15),
				FinalPrice = 09.99m,
				SubProductCodes = "",
				ProductCode = "TEST02",
			},
		});
		
		consumer = new ListSalesConsumer(mockLogger.Object, sales.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_OnePage()
	{
		var order = new ListSalesOrder(Pagination);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var data = responses.Single().Data?.Sales;
		
		Assert.That(data, Is.Not.Null);
		Assert.That(data!.Pagination.Count, Is.EqualTo(2));
		Assert.That(data!.Pagination.TotalCount, Is.EqualTo(2));
		Assert.That(data!.Pagination.TotalPages, Is.EqualTo(1));
		Assert.That(data!.Pagination.PageSize, Is.EqualTo(10));
		Assert.That(data!.Pagination.PageIndex, Is.EqualTo(0));
		
		Assert.That(data!.Items.Count(u => u.ProductCode == "TEST02"), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.ProductCode == "TEST01"), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => !u.SubProductCodes.Any()), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.SubProductCodes.Any(s => s == "SUB01")), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.SaleTime == "01/01/2000 00:00"), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.SaleTime == "01/01/2020 15:15"), Is.EqualTo(1));
	}
}
