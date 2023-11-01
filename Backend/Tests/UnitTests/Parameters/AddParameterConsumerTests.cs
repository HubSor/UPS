﻿using Consumers.Parameters;
using Dtos.Parameters;
using Helpers;
using Messages.Parameters;
using Models.Entities;
using NUnit.Framework;

namespace UnitTests.Parameters;

[TestFixture]
public class AddParameterConsumerTests : ConsumerTestCase<AddParameterConsumer, AddParameterOrder, AddParameterResponse>
{
	private MockRepository<Parameter> parameters = default!;
	private MockRepository<Product> products = default!;
	private MockRepository<SubProduct> subProducts = default!;

	protected override Task SetUp()
	{
		parameters = new MockRepository<Parameter>();
		products = new MockRepository<Product>();
		products.Entities.Add(new() 
		{
			Id = 1,
		});
		subProducts = new MockRepository<SubProduct>();
		subProducts.Entities.Add(new()
		{
			Id = 1,
		});

		consumer = new AddParameterConsumer(mockLogger.Object, parameters.Object, products.Object, subProducts.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_AddToProduct()
	{
		var order = new AddParameterOrder("test", true, ParameterTypeEnum.Text, 1, null, null);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newParam = parameters.Entities.SingleOrDefault();
		Assert.That(newParam, Is.Not.Null);
	
		Assert.That(newParam!.Name, Is.EqualTo("test"));
		Assert.That(newParam!.Required, Is.EqualTo(true));
		Assert.That(newParam!.Type, Is.EqualTo(ParameterTypeEnum.Text));
		Assert.That(newParam!.ProductId, Is.EqualTo(1));
		Assert.That(newParam!.SubProductId, Is.Null);
		Assert.That(newParam!.Options, Is.Empty);
	}

	[Test]
	public async Task Consume_Ok_AddToSubProduct()
	{
		var order = new AddParameterOrder("test2", true, ParameterTypeEnum.Select, null, 1, new List<OptionDto>(){ new() { Value = "option" } });

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		var newParam = parameters.Entities.SingleOrDefault();
		Assert.That(newParam, Is.Not.Null);

		Assert.That(newParam!.Name, Is.EqualTo("test2"));
		Assert.That(newParam!.Required, Is.EqualTo(true));
		Assert.That(newParam!.Type, Is.EqualTo(ParameterTypeEnum.Select));
		Assert.That(newParam!.SubProductId, Is.EqualTo(1));
		Assert.That(newParam!.ProductId, Is.Null);
		Assert.That(newParam!.Options, Is.Not.Empty);
	}
}
