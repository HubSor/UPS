using TestHelpers;
using NUnit.Framework;
using Core.Messages;
using Core.Models;
using SalesMicro.Consumers;
using Core.Dtos;
using Moq;
using MassTransit;
using Core.Web;

namespace UnitTests.Sales;

[TestFixture]
public class SaveSaleConsumerTests : ConsumerTestCase<SaveSaleConsumer, SaveSaleOrder, SaveSaleResponse>
{
	private MockRepository<Sale> sales = default!;
	private ExtendedProductDto productDto = default!;

	protected override Task SetUp()
	{
		sales = new MockRepository<Sale>();

		productDto = new ExtendedProductDto(new Product()
		{
			SubProductInProducts = [],
			Parameters = [],
		})
		{
			TaxRate = 0.5m,
			Code = "1"
		};

		ApiResponse<GetClientResponse> getClientResp = new();
		var mockResponse = new Mock<Response<ApiResponse<GetClientResponse>>>();
		mockResponse.Setup(x => x.Message).Returns(getClientResp);

		var mockGetClient = new Mock<IRequestClient<GetClientOrder>>();
		mockGetClient
			.Setup(x => x.GetResponse<ApiResponse<GetClientResponse>>(It.IsAny<GetClientOrder>(), CancellationToken.None, default))
			.ReturnsAsync(mockResponse.Object);

		ApiResponse<GetProductResponse> getProdResp = new() { Data = new() { Product = productDto } };
		var mockProdResponse = new Mock<Response<ApiResponse<GetProductResponse>>>();
		mockProdResponse.Setup(x => x.Message).Returns(getProdResp);

		var mockGetPRoduct = new Mock<IRequestClient<GetProductOrder>>();
		mockGetPRoduct
			.Setup(x => x.GetResponse<ApiResponse<GetProductResponse>>(It.IsAny<GetProductOrder>(), CancellationToken.None, default))
			.ReturnsAsync(mockProdResponse.Object);

		var mockSaveSale = new Mock<IRequestClient<SaveSaleProductsMicroOrder>>();

		consumer = new SaveSaleConsumer(mockLogger.Object, sales.Object, mockUnitOfWork.Object, mockGetClient.Object, mockGetPRoduct.Object, mockSaveSale.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_NoParametersNoClient()
	{
		var order = new SaveSaleOrder(1, null, Array.Empty<SaveSaleParameterDto>(), 99.99m, Array.Empty<SaveSaleSubProductDto>());
		
		var then = DateTime.Now;
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		var after = DateTime.Now;
		
		var newSale = sales.Entities.Single();
		Assert.That(newSale.FinalPrice, Is.EqualTo(99.99m));
		Assert.That(newSale.SellerId, Is.EqualTo(0));
		Assert.That(newSale.SaleTime, Is.InRange(then, after));
		Assert.That(newSale.ClientId, Is.Null);
	}
}