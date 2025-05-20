using Dtos;

namespace Messages.Queries;

public abstract record FindClientQuery(string Identifier);
public record FindPersonClientQuery(string Identifier) : FindClientQuery(Identifier);
public record FindCompanyClientQuery(string Identifier) : FindClientQuery(Identifier);
public abstract record ListClientsQuery(PaginationDto Pagination);
public record ListPersonClientsQuery(PaginationDto Pagination) : ListClientsQuery(Pagination);
public record ListCompanyClientsQuery(PaginationDto Pagination) : ListClientsQuery(Pagination);