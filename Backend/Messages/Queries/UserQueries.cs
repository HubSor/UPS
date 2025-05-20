using Dtos;

namespace Messages.Queries;

public record LoginQuery(string Username, string Password) : Query;
public record LogoutQuery() : Query;
public record SessionQuery() : Query;
public record ListUsersQuery(PaginationDto Pagination) : Query;