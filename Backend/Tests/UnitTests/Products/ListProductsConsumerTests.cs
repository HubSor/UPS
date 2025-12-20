using Dtos;
using TestHelpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;
using Services.Application;

namespace UnitTests.Products;

[TestFixture]
public class ListProductsConsumerTests : ServiceTestCase<ProductsApplicationService, ListProductsOrder, ListProductsResponse>
{
	private MockRepository<Product> products = default!;
	private readonly PaginationDto pagination = new (){ PageIndex = 0, PageSize = 10 };
	protected override Task SetUp()
	{
		products = new MockRepository<Product>();
		products.Entities.Add(new()
		{
			Id = 1,
			Name = "test",
			BasePrice = 99m,
			Status = ProductStatusEnum.NotOffered,
			AnonymousSaleAllowed = true,
			Code = "TEST1"
		});
		products.Entities.Add(new()
		{
			Id = 2,
			Name = "test2",
			BasePrice = 5.99m,
			Status = ProductStatusEnum.Offered,
			AnonymousSaleAllowed = false,
			Code = "TEST2"
		});

		service = new ProductsApplicationService(mockLogger.Object, mockUnitOfWork.Object, products.Object, GetMockRepo<SubProduct>(), GetMockRepo<SubProductInProduct>(), GetMockRepo<Parameter>());
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_ListSome()
	{
		var order = new ListProductsOrder(new ProductStatusEnum[] { ProductStatusEnum.NotOffered }, pagination);
		
		var response = await service.ListProductAsync(order);
		
		var product = response.Products.Items.FirstOrDefault();
		Assert.That(product, Is.Not.Null);
		Assert.That(product?.Id, Is.EqualTo(1));
		Assert.That(product?.Name, Is.EqualTo("test"));
		Assert.That(product?.Code, Is.EqualTo("TEST1"));
		Assert.That(product?.Status, Is.EqualTo(ProductStatusEnum.NotOffered));
	}
}
