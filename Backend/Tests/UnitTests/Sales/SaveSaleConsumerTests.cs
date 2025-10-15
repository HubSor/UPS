using TestHelpers;
using NUnit.Framework;
using Core.Messages;
using Core.Models;
using SalesMicro.Consumers;
using Core.Dtos;

namespace UnitTests.Sales;

[TestFixture]
public class SaveSaleConsumerTests : ConsumerTestCase<SaveSaleConsumer, ExtendedSaveSaleOrder, SaveSaleResponse>
{
	private MockRepository<Sale> sales = default!;
	private ExtendedProductDto productDto = default!;

	protected override Task SetUp()
	{
		sales = new MockRepository<Sale>();

		mockHttpContextAccessor.SetClaims(new User()
		{
			Id = 999,
			Name = "testuser",
			Roles = new List<Role>()
		});

		productDto = new ExtendedProductDto(new Product()
		{
			SubProductInProducts = [],
			Parameters = [],
		})
		{
			TaxRate = 0.5m,
			Code = "1"
		};

		consumer = new SaveSaleConsumer(mockLogger.Object, sales.Object, mockUnitOfWork.Object, mockHttpContextAccessor.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_NoParametersNoClient()
	{
		var order = new ExtendedSaveSaleOrder(1, null, Array.Empty<SaveSaleParameterDto>(), 99.99m, Array.Empty<SaveSaleSubProductDto>(), productDto, "test");
		
		var then = DateTime.Now;
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		var after = DateTime.Now;
		
		var newSale = sales.Entities.Single();
		Assert.That(newSale.FinalPrice, Is.EqualTo(99.99m));
		Assert.That(newSale.SellerId, Is.EqualTo(999));
		Assert.That(newSale.SaleTime, Is.InRange(then, after));
		Assert.That(newSale.ClientId, Is.Null);
	}
}