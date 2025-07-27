using Consumers.Query;
using Dtos;
using TestHelpers;
using Messages.Queries;
using Messages.Responses;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Sales;

[TestFixture]
public class ListSalesConsumerTests : ConsumerTestCase<ListSalesConsumer, ListSalesQuery, ListSalesResponse>
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
				Client = null,
				Product = new Product()
				{
					Id = 1,
					Code = "TEST01"
				},
				SubProducts = new List<SubProductInSale>(),
				FinalPrice = 99.99m
			},
			new ()
			{
				Id = 2,
				SaleTime = new DateTime(2020, 1, 1, 15, 15, 15),
				Client = new PersonClient()
				{
					FirstName = "Jan",
					LastName = "Łoś",
				},
				Product = new Product()
				{
					Id = 2,
					Code = "TEST02"
				},
				SubProducts = new List<SubProductInSale>() 
				{
					new()
					{
						SubProduct = new SubProduct()
						{
							Code = "SUB01"
						}
					}
				},
				FinalPrice = 09.99m
			},
		});
		
		consumer = new ListSalesConsumer(mockLogger.Object, sales.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_OnePage()
	{
		var order = new ListSalesQuery(Pagination);
		
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
