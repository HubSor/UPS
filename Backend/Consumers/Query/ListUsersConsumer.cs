using Core;
using Dtos;
using Dtos.Users;
using MassTransit;
using Messages.Queries;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Query;
public class ListUsersConsumer : BaseQueryConsumer<ListUsersQuery, ListUsersResponse>
{
	private readonly IRepository<User> users;
	
	public ListUsersConsumer(ILogger<ListUsersConsumer> logger, IRepository<User> users)
		: base(logger)
	{
		this.users = users;
	}

	public override async Task Consume(ConsumeContext<ListUsersQuery> context)
	{
		var userCount = users.GetAll().Count();
		var userList = await users.GetAll()
			.OrderBy(x => x.Id)
			.Include(u => u.Roles)
			.Skip(context.Message.Pagination.PageIndex * context.Message.Pagination.PageSize)
			.Take(context.Message.Pagination.PageSize)
			.Select(u => new UserDto()
			{
				Id = u.Id,
				Username = u.Name,
				Roles = u.Roles.Select(r => r.Id.ToString()).ToList()
			})
			.ToListAsync();
			
		await RespondAsync(context, new ListUsersResponse()
		{
			Users = new PagedList<UserDto>(userList, userCount,
				context.Message.Pagination.PageIndex, context.Message.Pagination.PageSize)
		});
		
		logger.LogInformation("Listing users");
	}
}
