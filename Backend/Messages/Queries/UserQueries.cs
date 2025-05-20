using Dtos;

namespace Messages.Queries;

public record LoginQuery(string Username, string Password);
public record LogoutQuery();
public record SessionQuery();
public record ListUsersQuery(PaginationDto Pagination);