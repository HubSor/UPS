using TestHelpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;
using Services.Application;

namespace UnitTests.Products;

[TestFixture]
public class UnassignSubProductsConsumerTests : ServiceTestCase<ProductsApplicationService, UnassignSubProductsOrder, UnassignSubProductsResponse>
{
	private MockRepository<Product> products = default!;
	private MockRepository<SubProduct> subProducts = default!;
	private MockRepository<SubProductInProduct> intersection = default!;

	protected override Task SetUp()
	{
		subProducts = new MockRepository<SubProduct>();
		subProducts.Entities.Add(new()
		{
			Id = 1,
		});
		subProducts.Entities.Add(new()
		{
			Id = 2,
		});
		
		products = new MockRepository<Product>();
		products.Entities.Add(new()
		{
			Id = 1
		});
		
		intersection = new MockRepository<SubProductInProduct>();
		intersection.Entities.Add(new()
		{
			SubProductId = 1,
			ProductId = 1,
			InProductPrice = 0.99m
		});
		intersection.Entities.Add(new()
		{
			SubProductId = 2,
			ProductId = 1,
			InProductPrice = 5.99m
		});

		service = new ProductsApplicationService(mockLogger.Object, mockUnitOfWork.Object, products.Object, subProducts.Object, intersection.Object, GetMockRepo<Parameter>());
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_SimpleUnassign()
	{
		var order = new UnassignSubProductsOrder(1, new int[] { 1 });
		
		await service.UnassignSubProductsAsync(order);
		
		var singleIntersection = intersection.Entities.SingleOrDefault();
		Assert.That(singleIntersection, Is.Not.Null);
		Assert.That(singleIntersection?.SubProductId, Is.EqualTo(2));
	}
	
	[Test]
	public async Task Consume_Ok_UnassignIdempotent()
	{
		intersection.Entities = intersection.Entities.Where(x => x.SubProductId != 1).ToList();
		var order = new UnassignSubProductsOrder(1, new int[] { 1 });
		
		await service.UnassignSubProductsAsync(order);
		
		var singleIntersection = intersection.Entities.SingleOrDefault();
		Assert.That(singleIntersection, Is.Not.Null);
		Assert.That(singleIntersection?.SubProductId, Is.EqualTo(2));
	}
}