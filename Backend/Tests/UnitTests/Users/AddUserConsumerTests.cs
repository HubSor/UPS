using Consumers.Users;
using Helpers;
using Messages.Users;
using Models.Entities;
using NUnit.Framework;
using Services;

namespace UnitTests.Users;

[TestFixture]
public class AddUserConsumerTests : ConsumerTestCase<AddUserConsumer, AddUserOrder, AddUserResponse>
{
	private static readonly string userPassword = "testowEha5ło";
	private readonly IPasswordService passwordService = new PasswordService();
	private MockRepository<User> users = default!;
	private MockRepository<Role> roles = default!;

	protected override Task SetUp()
	{
		users = new MockRepository<User>();
		users.Entities.Add(new()
		{
			Id = 123,
			Name = "test"
		});
		
		roles = new MockRepository<Role>();
		roles.Entities.Add(new()
		{
			Id = RoleEnum.Administrator,
			Description = "admin"
		});
		roles.Entities.Add(new()
		{
			Id = RoleEnum.Administrator,
			Description = "admin"
		});
			
		consumer = new AddUserConsumer(mockLogger.Object, users.Object, passwordService, roles.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_AddAdminAndSeller()
	{
		var order = new AddUserOrder("newUser", userPassword, new List<RoleEnum>(){ RoleEnum.Administrator, RoleEnum.Seller });
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newUser = users.Entities.SingleOrDefault(u => u.Name == "newUser");
		Assert.That(newUser, Is.Not.Null);
		
		var hash = passwordService.GenerateHash(userPassword, newUser!.Salt);
		Assert.That(newUser!.Hash, Is.EqualTo(hash));
		Assert.That(newUser!.Name, Is.EqualTo("newUser"));
		Assert.That(newUser!.Active, Is.True);
		Assert.That(newUser!.Roles, Has.Count.EqualTo(2));
	}
	
	[Test]
	public async Task Consume_Ok_AddNoRoles()
	{
		var order = new AddUserOrder("newUser", userPassword, new List<RoleEnum>(){ });
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newUser = users.Entities.SingleOrDefault(u => u.Name == "newUser");
		Assert.That(newUser, Is.Not.Null);
		
		var hash = passwordService.GenerateHash(userPassword, newUser!.Salt);
		Assert.That(newUser!.Hash, Is.EqualTo(hash));
		Assert.That(newUser!.Name, Is.EqualTo("newUser"));
		Assert.That(newUser!.Active, Is.True);
		Assert.That(newUser!.Roles, Has.Count.EqualTo(0));
	}
	
	[Test]
	public async Task Consume_BadRequest_UsernameTaken()
	{
		var order = new AddUserOrder("test", userPassword, new List<RoleEnum>(){ });
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}
