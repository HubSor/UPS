using TestHelpers;
using Messages.Users;
using Models.Entities;
using NUnit.Framework;
using Services;
using Services.Application;
using Services.Domain;
using Microsoft.AspNetCore.Identity;
using FluentValidation;

namespace UnitTests.Users;

[TestFixture]
public class LoginConsumerTests : ServiceTestCase<UsersApplicationService, LoginOrder, LoginResponse>
{
	private static readonly string userPassword = "testowEha5ło";
	private static readonly string userName= "admin";
	private static byte[] salt = default!;
	private static byte[] hash = default!;
	private readonly IPasswordService passwordService = new PasswordService();
	private MockRepository<User> users = default!;
	protected override Task OneTimeSetUp()
	{
		salt = passwordService.GenerateSalt();
		hash = passwordService.GenerateHash(userPassword, salt);
		return Task.CompletedTask;
	}

	protected override Task SetUp()
	{
		users = new MockRepository<User>();
		users.Entities.Add(new User()
		{
			Name = userName,
			Salt = salt,
			Hash = hash,
			Roles = new List<Role>()
			{
				new()
				{
					Id = RoleEnum.Administrator
				}
			}
		});

		service = new UsersApplicationService(mockLogger.Object, mockUnitOfWork.Object, users.Object, GetMockRepo<Role>(), mockHttpContextAccessor.Object, passwordService);
		return Task.CompletedTask;
	}
	
	[Test]
	public void Consume_BadRequest_NoUser()
	{
		var order = new LoginOrder("abc nie ma mnie w bazie", userPassword);
		
		Assert.ThrowsAsync<ValidationException>(() => service.LoginAsync(order));

		Assert.That(mockHttpContextAccessor.SignedIn, Is.False);
	}
	
	[Test]
	public void Consume_BadRequest_WrongPassword()
	{
		var order = new LoginOrder(userName, userPassword + "test");
		
		Assert.ThrowsAsync<ValidationException>(() => service.LoginAsync(order));

		Assert.That(mockHttpContextAccessor.SignedIn, Is.False);
	}
	
	[Test]
	public void Consume_BadRequest_InactiveUser()
	{
		users.Entities.Single().Active = false;
		var order = new LoginOrder(userName, userPassword);
		
		Assert.ThrowsAsync<ValidationException>(() => service.LoginAsync(order));

		Assert.That(mockHttpContextAccessor.SignedIn, Is.False);
	}
	
	[Test]
	public async Task Consume_Ok_GoodLogin()
	{
		var order = new LoginOrder(userName, userPassword);
		
		var result = (await service.LoginAsync(order))?.UserDto;
		Assert.That(mockHttpContextAccessor.SignedIn, Is.True);
		
		Assert.That(result, Is.Not.Null);
		Assert.That(result?.Username, Is.EqualTo(userName));
		Assert.That(result?.Roles, Is.EquivalentTo(new List<string>(){ "Administrator" }));
	}
}
