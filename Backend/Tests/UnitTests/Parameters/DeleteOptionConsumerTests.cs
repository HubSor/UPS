using TestHelpers;
using Messages.Parameters;
using Models.Entities;
using NUnit.Framework;
using Services.Application;

namespace UnitTests.Parameters;

[TestFixture]
public class DeleteOptionConsumerTests : ServiceTestCase<ParametersApplicationService, DeleteOptionOrder, DeleteOptionResponse>
{
	private MockRepository<ParameterOption> options = default!;

	protected override Task SetUp()
	{
		options = new MockRepository<ParameterOption>();
		options.Entities.Add(new ParameterOption()
		{
			Id = 1,
			Value = "test"
		});

		service = new ParametersApplicationService(mockLogger.Object, mockUnitOfWork.Object, GetMockRepo<Parameter>(), options.Object, GetMockRepo<Product>(), GetMockRepo<SubProduct>());
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_Delete()
	{
		var order = new DeleteOptionOrder(1);
		
		await service.DeleteOptionAsync(order); 

		Assert.That(options.Entities.Count, Is.EqualTo(0));
	}
}