using TestHelpers;
using Messages.Users;
using Models.Entities;
using NUnit.Framework;
using Services.Application;
using Microsoft.AspNetCore.Identity;
using Moq;
using Services.Domain;
using FluentValidation;

namespace UnitTests.Users;

[TestFixture]
public class DeleteUserConsumerTests : ServiceTestCase<UsersApplicationService, DeleteUserOrder, DeleteUserResponse>
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
		
		mockHttpContextAccessor.SetClaims(users.Entities.First(u => u.Id == 1));

		service = new UsersApplicationService(mockLogger.Object, mockUnitOfWork.Object, users.Object, GetMockRepo<Role>(), mockHttpContextAccessor.Object, new Mock<IPasswordService>().Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_DeleteSeller()
	{
		var order = new DeleteUserOrder(2);
		
		await service.DeleteUserAsync(order);
		
		Assert.That(users.Entities.Count, expression: Is.EqualTo(1));
		Assert.That(users.Entities.Single().Id, Is.EqualTo(1));
		Assert.That(users.Entities.Single().Roles.Single().Id, Is.EqualTo(RoleEnum.Administrator));
	}
	
	[Test]
	public void Consume_BadRequest_DeleteSelf()
	{
		var order = new DeleteUserOrder(1);
		
		Assert.ThrowsAsync<ValidationException>(() => service.DeleteUserAsync(order));
		
		Assert.That(users.Entities.Count, expression: Is.EqualTo(2));
	}

	[Test]
	public void Consume_BadRequest_DeleteAdminAsManager()
	{
		mockHttpContextAccessor.SetClaims(users.Entities.First(u => u.Id == 2));
		var order = new DeleteUserOrder(1);
		
		Assert.ThrowsAsync<ValidationException>(() => service.DeleteUserAsync(order));

		Assert.That(users.Entities.Count, expression: Is.EqualTo(2));
	}
}