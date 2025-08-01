﻿using Consumers.Products;
using TestHelpers;
using Messages.Products;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Products;

[TestFixture]
public class DeleteProductConsumerTests : ConsumerTestCase<DeleteProductConsumer, DeleteProductOrder, DeleteProductResponse>
{
	private MockRepository<Parameter> parameters = default!;
	private MockRepository<Product> products = default!;
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

		products = new MockRepository<Product>();
		products.Entities.Add(new()
		{
			Id = 1,
			SubProductInProducts = new List<SubProductInProduct>(){ subProductInProduct },
			Parameters = new List<Parameter>(){ param },
			Sales = new List<Sale>()
		});
		
		parameters = new MockRepository<Parameter>();
		parameters.Entities.Add(param);
		
		intersection = new MockRepository<SubProductInProduct>();
		intersection.Entities.Add(subProductInProduct);

		consumer = new DeleteProductConsumer(mockLogger.Object, products.Object,
			intersection.Object, parameters.Object,  mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_HardDelete()
	{
		var order = new DeleteProductOrder(1);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		Assert.That(intersection.Entities.Count, Is.EqualTo(0));
		Assert.That(products.Entities.Count, Is.EqualTo(0));
		Assert.That(parameters.Entities.Count, Is.EqualTo(0));
	}

	[Test]
	public async Task Consume_Ok_SoftDeleteSale()
	{
		products.Entities.Single().Sales = new List<Sale>()
		{
			new()
			{
				ProductId = 1,
				Id = 1
			}	
		};
		var order = new DeleteProductOrder(1);

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		Assert.That(intersection.Entities.Count, Is.EqualTo(0));
		Assert.That(products.Entities.Count, Is.EqualTo(1));
		Assert.That(products.Entities.Single().Deleted, Is.EqualTo(true));
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
		var order = new DeleteProductOrder(1);

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		Assert.That(intersection.Entities.Count, Is.EqualTo(0));
		Assert.That(products.Entities.Count, Is.EqualTo(1));
		Assert.That(products.Entities.Single().Deleted, Is.EqualTo(true));
		Assert.That(parameters.Entities.Count, Is.EqualTo(1));
		Assert.That(parameters.Entities.Single().Deleted, Is.EqualTo(true));
	}
}