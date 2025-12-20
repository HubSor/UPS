using TestHelpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;
using Services.Application;

namespace UnitTests.Products;

[TestFixture]
public class GetProductConsumerTests : ServiceTestCase<ProductsApplicationService, GetProductOrder, GetProductResponse>
{
	private MockRepository<Product> products = default!;

	protected override Task SetUp()
	{
		var subProductInProduct1 = new SubProductInProduct()
		{
			SubProduct = new SubProduct()
			{
				Id = 1,
				Name = "test1",
				Code = "CODE1",
				BasePrice = 99,
				Parameters = Array.Empty<Parameter>()
			}	
		};
		var subProductInProduct2 = new SubProductInProduct()
		{
			SubProduct = new SubProduct()
			{
				Id = 2,
				Name = "test2",
				Code = "CODE2",
				BasePrice = 9.99m,
				Parameters = Array.Empty<Parameter>()
			}
		};

		products = new MockRepository<Product>();
		products.Entities.Add(new()
		{
			Id = 1,
			SubProductInProducts = new List<SubProductInProduct>(){ subProductInProduct1, subProductInProduct2 },
			Name = "produkt",
			Code = "CODE",
			BasePrice = 99.65m,
			AnonymousSaleAllowed = true,
			Status = ProductStatusEnum.Offered,
			Parameters = new List<Parameter>()
			{
				new ()
				{
					Id = 1,
					Name = "param",
					Type = ParameterTypeEnum.Decimal,
					Options = new List<ParameterOption>(),
				}
			}
		});

		service = new ProductsApplicationService(mockLogger.Object, mockUnitOfWork.Object, products.Object, GetMockRepo<SubProduct>(), GetMockRepo<SubProductInProduct>(), GetMockRepo<Parameter>());
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_GetWithSubProducts()
	{
		var order = new GetProductOrder(1);
		
		var product = (await service.GetProductAsync(order)).Product;
		
		Assert.That(product, Is.Not.Null);
		Assert.That(product!.Id, Is.EqualTo(1));
		Assert.That(product!.Code, Is.EqualTo("CODE"));
		Assert.That(product!.SubProducts.Count, Is.EqualTo(2));
		Assert.That(product!.SubProducts.Any(x => x.Id == 2), Is.True);
		Assert.That(product!.SubProducts.Any(x => x.Id == 1), Is.True);
		Assert.That(product!.SubProducts.Any(x => x.Code == "CODE1"), Is.True);
		Assert.That(product!.SubProducts.Any(x => x.Code == "CODE2"), Is.True);
		Assert.That(product!.Parameters.Any(x => x.Name == "param"), Is.True);
	}
}