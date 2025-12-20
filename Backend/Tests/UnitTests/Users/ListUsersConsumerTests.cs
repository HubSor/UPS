using Dtos;
using TestHelpers;
using Messages.Users;
using Models.Entities;
using NUnit.Framework;
using Services.Application;
using Moq;
using Services.Domain;

namespace UnitTests.Users;

[TestFixture]
public class ListUsersConsumerTests : ServiceTestCase<UsersApplicationService, ListUsersOrder, ListUsersResponse>
{
	private PaginationDto Pagination = default!;
	private MockRepository<User> users = default!;
	
	protected override Task SetUp()
	{
		Pagination = new();
		
		users = new MockRepository<User>();
		users.Entities.AddRange(new List<User>()
		{
			new ()
			{
				Id = 1,
				Name = "first",
				Roles = new List<Role>()
				{
					new()
					{
						Id = RoleEnum.Administrator
					}
				}
			},
			new ()
			{
				Id = 2,
				Name = "second",
				Roles = new List<Role>()
				{
					new()
					{
						Id = RoleEnum.UserManager
					},
					new()
					{
						Id = RoleEnum.Seller
					}
				}
			},
			new ()
			{
				Id = 3,
				Name = "third",
				Roles = new List<Role>(){}
			},
		});
		
		service = new UsersApplicationService(mockLogger.Object, mockUnitOfWork.Object, users.Object, GetMockRepo<Role>(), mockHttpContextAccessor.Object, new Mock<IPasswordService>().Object);
		return Task.CompletedTask;
	}
	
	[Test]
	public async Task Consume_Ok_OnePage()
	{
		var order = new ListUsersOrder(Pagination);
		
		var data = (await service.ListUsersAsync(order))?.Users;
		
		Assert.That(data, Is.Not.Null);
		Assert.That(data!.Pagination.Count, Is.EqualTo(3));
		Assert.That(data!.Pagination.TotalCount, Is.EqualTo(3));
		Assert.That(data!.Pagination.TotalPages, Is.EqualTo(1));
		Assert.That(data!.Pagination.PageSize, Is.EqualTo(10));
		Assert.That(data!.Pagination.PageIndex, Is.EqualTo(0));
		
		Assert.That(data!.Items.Count(u => u.Username == "first"), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.Username == "second"), Is.EqualTo(1));
		Assert.That(data!.Items.Count(u => u.Username == "third"), Is.EqualTo(1));
		Assert.That(data!.Items.First(u => u.Username == "second").Roles.Contains("UserManager"), Is.True);
	}
	
	[Test]
	public async Task Consume_Ok_TwoPages()
	{
		Pagination.PageSize = 2;
		Pagination.PageIndex = 1;
		var order = new ListUsersOrder(Pagination);
		
		var data = (await service.ListUsersAsync(order))?.Users;
		
		Assert.That(data, Is.Not.Null);
		Assert.That(data!.Pagination.Count, Is.EqualTo(1));
		Assert.That(data!.Pagination.TotalCount, Is.EqualTo(3));
		Assert.That(data!.Pagination.TotalPages, Is.EqualTo(2));
		Assert.That(data!.Pagination.PageSize, Is.EqualTo(2));
		Assert.That(data!.Pagination.PageIndex, Is.EqualTo(1));
		
		Assert.That(data!.Items.Count(u => u.Username == "first"), Is.EqualTo(0));
		Assert.That(data!.Items.Count(u => u.Username == "second"), Is.EqualTo(0));
		Assert.That(data!.Items.Count(u => u.Username == "third"), Is.EqualTo(1));
	}
	
	[Test]
	public async Task Consume_Ok_Empty()
	{
		users.Entities.Clear();
		var order = new ListUsersOrder(Pagination);
		
		var data = (await service.ListUsersAsync(order))?.Users;
		
		Assert.That(data, Is.Not.Null);
		Assert.That(data!.Pagination.Count, Is.EqualTo(0));
		Assert.That(data!.Pagination.TotalCount, Is.EqualTo(0));
		Assert.That(data!.Pagination.TotalPages, Is.EqualTo(0));
		Assert.That(data!.Pagination.PageSize, Is.EqualTo(10));
		Assert.That(data!.Pagination.PageIndex, Is.EqualTo(0));
		
		Assert.That(data!.Items.Any(), Is.False);
	}
}
