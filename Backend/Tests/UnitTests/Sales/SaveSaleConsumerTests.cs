using TestHelpers;
using NUnit.Framework;

namespace UnitTests.Sales;

[TestFixture]
public class SaveSaleConsumerTests : ConsumerTestCase<SaveSaleConsumer, SaveSaleOrder, SaveSaleResponse>
{
	private MockRepository<Sale> sales = default!;
	private MockRepository<Product> products = default!;
	private MockRepository<Parameter> parameters = default!;
	private MockRepository<Client> clients = default!;

	protected override Task SetUp()
	{
		sales = new MockRepository<Sale>();
		parameters = new MockRepository<Parameter>();
		parameters.Entities.Add(new Parameter()
		{
			Id = 3,
			Type = ParameterTypeEnum.Integer,
			Required = true,
			ProductId = 1,
			Options = Array.Empty<ParameterOption>()
		});
		parameters.Entities.Add(new Parameter()
		{
			Id = 33,
			Type = ParameterTypeEnum.Select,
			Required = false,
			ProductId = 1,
			Options = new List<ParameterOption>()
			{
				new ()
				{
					Id = 700,
					Value = "test"
				},
				new () 
				{
					Id = 900,
					Value = "test2"
				}
			}
		});

		products = new MockRepository<Product>();
		products.Entities.Add(new()
		{
			Id = 1,
			SubProductInProducts = new List<SubProductInProduct>(),
			Status = ProductStatusEnum.Offered,
			Parameters = parameters.Entities
		});
		
		clients = new MockRepository<Client>();
		clients.Entities.Add(new PersonClient() 
		{
			Id = 500,
			FirstName = "Janusz",
			LastName = "Łoś",
			PhoneNumber = "123456789"
		});
		
		mockHttpContextAccessor.SetClaims(new User() 
		{
			Id = 999,
			Name = "testuser",
			Roles = new List<Role>()
		});

		consumer = new SaveSaleConsumer(mockLogger.Object, sales.Object, parameters.Object, products.Object, clients.Object, mockUnitOfWork.Object, mockHttpContextAccessor.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_NoParametersNoClient()
	{
		parameters.Entities.Clear();
		var order = new SaveSaleOrder(1, null, Array.Empty<SaveSaleParameterDto>(), 99.99m, Array.Empty<SaveSaleSubProductDto>());
		
		var then = DateTime.Now;
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		var after = DateTime.Now;
		
		var newSale = sales.Entities.Single();
		Assert.That(newSale.FinalPrice, Is.EqualTo(99.99m));
		Assert.That(newSale.SellerId, Is.EqualTo(999));
		Assert.That(newSale.SaleTime, Is.InRange(then, after));
		Assert.That(newSale.SubProducts, Is.Empty);
		Assert.That(newSale.SaleParameters, Is.Empty);
		Assert.That(newSale.ClientId, Is.Null);
	}

	[Test]
	public async Task Consume_Ok_ClientAndNoParameters()
	{
		parameters.Entities.Clear();
		var order = new SaveSaleOrder(1, 500, Array.Empty<SaveSaleParameterDto>(), 99.99m, Array.Empty<SaveSaleSubProductDto>());

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		var newSale = sales.Entities.Single();
		Assert.That(newSale.FinalPrice, Is.EqualTo(99.99m));
		Assert.That(newSale.SellerId, Is.EqualTo(999));
		Assert.That(newSale.SubProducts, Is.Empty);
		Assert.That(newSale.SaleParameters, Is.Empty);
		Assert.That(newSale.ClientId, Is.EqualTo(500));
	}

	[Test]
	public async Task Consume_Ok_ParametersAndClient()
	{
		var order = new SaveSaleOrder(1, 500, new SaveSaleParameterDto[] 
		{
			new ()
			{
				ParameterId = 3,
				Answer = "10"
			}
		}, 99.99m, Array.Empty<SaveSaleSubProductDto>());

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		var newSale = sales.Entities.Single();
		Assert.That(newSale.FinalPrice, Is.EqualTo(99.99m));
		Assert.That(newSale.SellerId, Is.EqualTo(999));
		Assert.That(newSale.SubProducts, Is.Empty);
		Assert.That(newSale.SaleParameters.Single().ParameterId, Is.EqualTo(3));
		Assert.That(newSale.SaleParameters.Single().Value, Is.EqualTo("10"));
		Assert.That(newSale.ClientId, Is.EqualTo(500));
	}

	[Test]
	public async Task Consume_Ok_ParameterOption()
	{
		var order = new SaveSaleOrder(1, 500, new SaveSaleParameterDto[]
		{
			new ()
			{
				ParameterId = 33,
				Answer = "test2"
			},
			new ()
			{
				ParameterId = 3,
				Answer = "79"
			}
		}, 99.99m, Array.Empty<SaveSaleSubProductDto>());

		await consumer.Consume(GetConsumeContext(order));
		AssertOk();

		var newSale = sales.Entities.Single();
		Assert.That(newSale.FinalPrice, Is.EqualTo(99.99m));
		Assert.That(newSale.SellerId, Is.EqualTo(999));
		Assert.That(newSale.SubProducts, Is.Empty);
		Assert.That(newSale.SaleParameters.Single(x => x.ParameterId == 33).Value, Is.EqualTo("test2"));
		Assert.That(newSale.SaleParameters.Single(x => x.ParameterId == 33).OptionId, Is.EqualTo(900));
		Assert.That(newSale.ClientId, Is.EqualTo(500));
	}

	[Test]
	public async Task Consume_BadRequest_NoRequiredAnswer()
	{
		var order = new SaveSaleOrder(1, 500, new SaveSaleParameterDto[]
		{
			new ()
			{
				ParameterId = 33,
				Answer = "10"
			}
		}, 99.99m, Array.Empty<SaveSaleSubProductDto>());

		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}

	[Test]
	public async Task Consume_BadRequest_InvalidValue()
	{
		var order = new SaveSaleOrder(1, 500, new SaveSaleParameterDto[]
		{
			new ()
			{
				ParameterId = 3,
				Answer = "test"
			}
		}, 99.99m, Array.Empty<SaveSaleSubProductDto>());

		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}

	[Test]
	public async Task Consume_BadRequest_DuplicatedAnswer()
	{
		var order = new SaveSaleOrder(1, 500, new SaveSaleParameterDto[]
		{
			new ()
			{
				ParameterId = 3,
				Answer = "4"
			},
			new ()
			{
				ParameterId = 3,
				Answer = "3"
			}
		}, 99.99m, Array.Empty<SaveSaleSubProductDto>());

		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}