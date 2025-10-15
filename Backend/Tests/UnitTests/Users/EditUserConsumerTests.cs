using TestHelpers;
using NUnit.Framework;
using UsersMicro.Consumers;
using Core.Messages;
using UsersMicro.Services;
using Core.Models;

namespace UnitTests.Users;

[TestFixture]
public class EditUserConsumerTests : ConsumerTestCase<EditUserConsumer, EditUserOrder, EditUserResponse>
{
	private static readonly string userPassword = "testowEha5ło";
	private readonly IPasswordService passwordService = new PasswordService();
	private MockRepository<User> users = default!;
	private MockRepository<Role> roles = default!;

	protected override Task SetUp()
	{
		var salt = passwordService.GenerateSalt();
		var hash = passwordService.GenerateHash(userPassword, salt);
		
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
			},
			Salt = salt,
			Hash = hash
		});
		users.Entities.Add(new()
		{
			Id = 2,
			Name = "edited",
			Roles = new List<Role>()
			{
				new()
				{
					Id = RoleEnum.Seller,
				}
			},
			Salt = salt,
			Hash = hash
		});
		
		roles = new MockRepository<Role>();
		roles.Entities.Add(new()
		{
			Id = RoleEnum.Administrator,
			Description = "admin"
		});
		roles.Entities.Add(new()
		{
			Id = RoleEnum.Seller,
			Description = "seller"
		});
		
		mockHttpContextAccessor.SetClaims(users.Entities.First(u => u.Id == 1));
			
		consumer = new EditUserConsumer(mockHttpContextAccessor.Object, mockLogger.Object, 
			users.Object, passwordService, roles.Object, mockUnitOfWork.Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_ChangeAll()
	{
		var order = new EditUserOrder(2, "newUser", userPassword + "eee", new List<RoleEnum>(){ RoleEnum.Administrator, RoleEnum.Seller });
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newUser = users.Entities.FirstOrDefault(u => u.Name == "newUser");
		Assert.That(newUser, Is.Not.Null);
		
		var hash = passwordService.GenerateHash(userPassword + "eee", newUser!.Salt);
		Assert.That(newUser!.Hash, Is.EqualTo(hash));
		Assert.That(newUser!.Name, Is.EqualTo("newUser"));
		Assert.That(newUser!.Roles, Has.Count.EqualTo(2));
	}
	
	[Test]
	public async Task Consume_Ok_UnchangedPassword()
	{
		var order = new EditUserOrder(2, "newUser", null, new List<RoleEnum>(){ RoleEnum.Administrator });
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newUser = users.Entities.FirstOrDefault(u => u.Name == "newUser");
		Assert.That(newUser, Is.Not.Null);
		
		var hash = passwordService.GenerateHash(userPassword, newUser!.Salt);
		Assert.That(newUser!.Hash, Is.EqualTo(hash));
		Assert.That(newUser!.Name, Is.EqualTo("newUser"));
		Assert.That(newUser!.Roles, Has.Count.EqualTo(1));
	}
	
	[Test]
	public async Task Consume_Ok_EditingSelfUnchangedPassword()
	{
		var order = new EditUserOrder(1, "newUser", null, new List<RoleEnum>(){ RoleEnum.Administrator });
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var newUser = users.Entities.FirstOrDefault(u => u.Name == "newUser");
		Assert.That(newUser, Is.Not.Null);
		
		var hash = passwordService.GenerateHash(userPassword, newUser!.Salt);
		Assert.That(newUser!.Hash, Is.EqualTo(hash));
		Assert.That(newUser!.Name, Is.EqualTo("newUser"));
		Assert.That(newUser!.Roles, Has.Count.EqualTo(1));
	}
	
	[Test]
	public async Task Consume_BadRequest_NotAdminEditingSelt()
	{
		mockHttpContextAccessor.SetClaims(users.Entities.First(u => u.Id == 2));
			
		var order = new EditUserOrder(2, "newUser", null, new List<RoleEnum>(){ RoleEnum.Administrator });
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
	
	[Test]
	public async Task Consume_BadRequest_AdminRemovingAdmin()
	{
		mockHttpContextAccessor.SetClaims(users.Entities.First(u => u.Id == 1));
			
		var order = new EditUserOrder(1, "newUser", null, new List<RoleEnum>(){ });
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
}
