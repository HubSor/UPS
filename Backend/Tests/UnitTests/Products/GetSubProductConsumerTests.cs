using Consumers.Products;
using TestHelpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Products;

[TestFixture]
public class GetSubProductConsumerTests : ServiceTestCase<GetSubProductConsumer, GetSubProductOrder, GetSubProductResponse>
{
	private MockRepository<SubProduct> subProducts = default!;

	protected override Task SetUp()
	{
		var subProductInProduct1 = new SubProductInProduct()
		{
			Product = new Product()
			{
				Id = 1,
				Name = "test1",
				Code = "CODE1",
				BasePrice = 99,
				Status = ProductStatusEnum.Offered
			}	
		};
		var subProductInProduct2 = new SubProductInProduct()
		{
			Product = new Product()
			{
				Id = 2,
				Name = "test2",
				Code = "CODE2",
				BasePrice = 9.99m,
				Status = ProductStatusEnum.Withdrawn
			}
		};

		subProducts = new MockRepository<SubProduct>();
		subProducts.Entities.Add(new()
		{
			Id = 1,
			SubProductInProducts = new List<SubProductInProduct>(){ subProductInProduct1, subProductInProduct2 },
			Name = "podprodukt",
			Code = "CODE",
			BasePrice = 99.65m,
			Parameters = new List<Parameter>()
			{
				new ()
				{
					Id = 1,
					Name = "param",
					Type = ParameterTypeEnum.Select,
					Options = new List<ParameterOption>()
					{
						new()
						{
							Value = "1"
						},
						new ()
						{
							Value = "2"
						}
					},
				}
			}
		});

		consumer = new GetSubProductConsumer(mockLogger.Object, subProducts.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_GetWithProducts()
	{
		var order = new GetSubProductOrder(1);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var product = responses.Single().Data?.SubProduct;
		Assert.That(product, Is.Not.Null);
		Assert.That(product!.Id, Is.EqualTo(1));
		Assert.That(product!.Code, Is.EqualTo("CODE"));
		Assert.That(product!.Products.Count, Is.EqualTo(2));
		Assert.That(product!.Products.Any(x => x.Id == 2), Is.True);
		Assert.That(product!.Products.Any(x => x.Id == 1), Is.True);
		Assert.That(product!.Products.Any(x => x.Code == "CODE1"), Is.True);
		Assert.That(product!.Products.Any(x => x.Code == "CODE2"), Is.True);
		Assert.That(product!.Parameters.Any(x => x.Name == "param"), Is.True);
		Assert.That(product!.Parameters.Single(x => x.Name == "param").Options.Count, Is.EqualTo(2));
	}
}