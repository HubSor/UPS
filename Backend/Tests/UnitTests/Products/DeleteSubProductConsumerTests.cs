using TestHelpers;
using NUnit.Framework;

namespace UnitTests.Products;

[TestFixture]
public class DeleteSubProductConsumerTests : ConsumerTestCase<DeleteSubProductConsumer, DeleteSubProductOrder, DeleteSubProductResponse>
{
	private MockRepository<Parameter> parameters = default!;
	private MockRepository<SubProduct> subProducts = default!;
	private MockRepository<SubProductInProduct> intersection = default!;

	protected override Task SetUp()
	{
		var subProductInProduct = new SubProductInProduct()
		{
			ProductId = 1,
			SubProductId = 1,
		};
		
		var param = new Parameter()
		{
			Id = 1,
			SaleParameters = new List<SaleParameter>()
		};

		subProducts = new MockRepository<SubProduct>();
		subProducts.Entities.Add(new()
		{
			Id = 1,
			SubProductInProducts = new List<SubProductInProduct>(){ subProductInProduct },
			Parameters = new List<Parameter>(){ param },
			SubProductInSales = new List<SubProductInSale>()
		});
		
		parameters = new MockRepository<Parameter>();
		parameters.Entities.Add(param);
		
		intersection = new MockRepository<SubProductInProduct>();
		intersection.Entities.Add(subProductInProduct);

		consumer = new DeleteSubProductConsumer(mockLogger.Object, subProducts.Object,
			intersection.Object, parameters.Object,  mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_HardDelete()
	{
		var order = new DeleteSubProductOrder(1);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		Assert.That(intersection.Entities.Count, Is.EqualTo(0));
		Assert.That(subProducts.Entities.Count, Is.EqualTo(0));
		Assert.That(parameters.Entities.Count, Is.EqualTo(0));
	}

	[Test]
	public async Task Consume_Ok_SoftDeleteSale()
	{
		subProducts.Entities.Single().SubProductInSales = new List<SubProductInSale>()
		{
			new()
			{
				SubProductId = 1,
				SaleId = 1
			}	
		};
		var order = new DeleteSubProductOrder(1);

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		Assert.That(intersection.Entities.Count, Is.EqualTo(0));
		Assert.That(subProducts.Entities.Count, Is.EqualTo(1));
		Assert.That(subProducts.Entities.Single().Deleted, Is.EqualTo(true));
		Assert.That(parameters.Entities.Count, Is.EqualTo(0));
	}

	[Test]
	public async Task Consume_Ok_SoftDeleteParam()
	{
		parameters.Entities.Single().SaleParameters = new List<SaleParameter>()
		{
			new()
			{
				SaleId = 1,
				ParameterId = 1
			}
		};
		var order = new DeleteSubProductOrder(1);

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		Assert.That(intersection.Entities.Count, Is.EqualTo(0));
		Assert.That(subProducts.Entities.Count, Is.EqualTo(1));
		Assert.That(subProducts.Entities.Single().Deleted, Is.EqualTo(true));
		Assert.That(parameters.Entities.Count, Is.EqualTo(1));
		Assert.That(parameters.Entities.Single().Deleted, Is.EqualTo(true));
	}
}