using TestHelpers;
using Messages.Users;
using Models.Entities;
using NUnit.Framework;
using Services;
using Services.Application;
using Services.Domain;
using FluentValidation;

namespace UnitTests.Users;

[TestFixture]
public class AddUserConsumerTests : ServiceTestCase<UsersApplicationService, AddUserOrder, AddUserResponse>
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
			Id = RoleEnum.Seller,
			Description = "seller"
		});
			
		service = new UsersApplicationService(mockLogger.Object, mockUnitOfWork.Object, users.Object, roles.Object, mockHttpContextAccessor.Object, passwordService);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_AddAdminAndSeller()
	{
		var order = new AddUserOrder("newUser", userPassword, new List<RoleEnum>(){ RoleEnum.Administrator, RoleEnum.Seller });		
		await service.AddUserAsync(order);
		
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
		await service.AddUserAsync(order);

		var newUser = users.Entities.SingleOrDefault(u => u.Name == "newUser");
		Assert.That(newUser, Is.Not.Null);
		
		var hash = passwordService.GenerateHash(userPassword, newUser!.Salt);
		Assert.That(newUser!.Hash, Is.EqualTo(hash));
		Assert.That(newUser!.Name, Is.EqualTo("newUser"));
		Assert.That(newUser!.Active, Is.True);
		Assert.That(newUser!.Roles, Has.Count.EqualTo(0));
	}
	
	[Test]
	public void Consume_BadRequest_UsernameTaken()
	{
		var order = new AddUserOrder("test", userPassword, new List<RoleEnum>(){ });
		
		Assert.ThrowsAsync<ValidationException>(() => service.AddUserAsync(order));
	}
}
