using Dtos;

namespace Messages.Queries;

public abstract record FindClientQuery(string Identifier) : Query;
public record FindPersonClientQuery(string Identifier) : FindClientQuery(Identifier);
public record FindCompanyClientQuery(string Identifier) : FindClientQuery(Identifier);
public abstract record ListClientsQuery(PaginationDto Pagination) : Query;
public record ListPersonClientsQuery(PaginationDto Pagination) : ListClientsQuery(Pagination);
public record ListCompanyClientsQuery(PaginationDto Pagination) : ListClientsQuery(Pagination);