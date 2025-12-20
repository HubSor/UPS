using Dtos;
using TestHelpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;
using Services.Application;

namespace UnitTests.Products;

[TestFixture]
public class ListSubProductsConsumerTests : ServiceTestCase<ProductsApplicationService, ListSubProductsOrder, ListSubProductsResponse>
{
	private MockRepository<Product> products = default!;
	private MockRepository<SubProduct> subProducts = default!;
	private readonly PaginationDto pagination = new() { PageIndex = 0, PageSize = 10 };

	protected override Task SetUp()
	{
		products = new MockRepository<Product>();
		products.Entities.Add(new()
		{
			Id = 1
		});
		
		subProducts = new MockRepository<SubProduct>();
		subProducts.Entities.Add(new()
		{
			Id = 1,
			Name = "test",
			BasePrice = 0.99m,
			Code = "T1",
			SubProductInProducts = new List<SubProductInProduct>()
			{
				new()
				{
					SubProductId = 1,
					ProductId = 1,
				}
			}
		});
		subProducts.Entities.Add(new()
		{
			Id = 2,
			Name = "test2",
			BasePrice = 2.99m,
			Code = "T2",
			SubProductInProducts = new List<SubProductInProduct>()
		});

		service = new ProductsApplicationService(mockLogger.Object, mockUnitOfWork.Object, products.Object, subProducts.Object, GetMockRepo<SubProductInProduct>(), GetMockRepo<Parameter>());
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_WithoutProductId()
	{
		var order = new ListSubProductsOrder(null, pagination);
		
		var resp = await service.ListSubProductsAsync(order);
		
		var result = resp.SubProducts;
		Assert.That(result, Is.Not.Null);
		Assert.That(result?.Items, Has.Count.EqualTo(2));
	}
	
	[Test]
	public async Task Consume_Ok_WithProductId()
	{
		var order = new ListSubProductsOrder(1, pagination);
		
		var resp = await service.ListSubProductsAsync(order);
		
		var result = resp.SubProducts;
		Assert.That(result, Is.Not.Null);
		Assert.That(result?.Items, Has.Count.EqualTo(1));
		Assert.That(result?.Items.Single().Id, Is.EqualTo(2));
	}
}
