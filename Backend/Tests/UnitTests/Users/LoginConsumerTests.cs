using TestHelpers;
using NUnit.Framework;
using UsersMicro.Consumers;
using Core.Messages;
using UsersMicro.Services;
using Core.Models;

namespace UnitTests.Users;

[TestFixture]
public class LoginConsumerTests : ConsumerTestCase<LoginConsumer, LoginOrder, LoginResponse>
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
		consumer = new LoginConsumer(mockUnitOfWork.Object, users.Object, mockLogger.Object, passwordService);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_BadRequest_NoUser()
	{
		var order = new LoginOrder("abc nie ma mnie w bazie", userPassword);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
		Assert.That(responses.Single().Errors!.Values.Any(v => v.Contains("Niepoprawne hasło")), Is.True);
	}
	
	[Test]
	public async Task Consume_BadRequest_WrongPassword()
	{
		var order = new LoginOrder(userName, userPassword + "test");
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
	
	[Test]
	public async Task Consume_BadRequest_InactiveUser()
	{
		users.Entities.Single().Active = false;
		var order = new LoginOrder(userName, userPassword);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertBadRequest();
	}
	
	[Test]
	public async Task Consume_Ok_GoodLogin()
	{
		var order = new LoginOrder(userName, userPassword);
		
		await consumer.Consume(GetConsumeContext(order));
		AssertOk();
		
		var result = responses.Single().Data?.UserDto;
		Assert.That(result, Is.Not.Null);
		Assert.That(result?.Username, Is.EqualTo(userName));
		Assert.That(result?.Roles, Is.EquivalentTo(new List<string>(){ "Administrator" }));
	}
}
