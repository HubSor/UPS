using TestHelpers;
using NUnit.Framework;
using UsersMicro.Consumers;
using Core.Messages;
using Core.Models;

namespace UnitTests.Users;

[TestFixture]
public class DeleteUserConsumerTests : ConsumerTestCase<DeleteUserConsumer, DeleteUserOrder, DeleteUserResponse>
{
	private MockRepository<User> users = default!;

	protected override Task SetUp()
	{
		users = new MockRepository<User>();
		users.Entities.Add(new()
		{
			Id = 1,
			Name = "loggedIn",
			Roles = new List<Role>()
			{
				new()
				{
					Id = RoleEnum.Administrator,
				}
			}
		});
		users.Entities.Add(new()
		{
			Id = 2,
			Name = "Deleteed",
			Roles = new List<Role>()
			{
				new()
				{
					Id = RoleEnum.Seller,
				}
			}
		});
			
		consumer = new DeleteUserConsumer(mockLogger.Object, users.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_DeleteSeller()
	{
		var order = new DeleteUserOrder(2);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		Assert.That(users.Entities.Count, expression: Is.EqualTo(1));
		Assert.That(users.Entities.Single().Id, Is.EqualTo(1));
		Assert.That(users.Entities.Single().Roles.Single().Id, Is.EqualTo(RoleEnum.Administrator));
	}
	
	[Test]
	public async Task Consume_BadRequest_DeleteSelf()
	{
		var order = new DeleteUserOrder(1);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
		
		Assert.That(users.Entities.Count, expression: Is.EqualTo(2));
	}
	
	[Test]
	public async Task Consume_BadRequest_DeleteAdminAsManager()
	{;
		var order = new DeleteUserOrder(1);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
		
		Assert.That(users.Entities.Count, expression: Is.EqualTo(2));
	}
}