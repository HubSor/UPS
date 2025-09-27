using Dtos;

namespace UsersMicro.Messages;

public abstract record FindClientOrder(string Identifier);
public record FindPersonClientOrder(string Identifier) : FindClientOrder(Identifier);
public record FindCompanyClientOrder(string Identifier) : FindClientOrder(Identifier);
public record UpsertClientOrder(bool IsCompany, int? ClientId, string? PhoneNumber, string? Email, string? FirstName,
	string? LastName, string? Pesel, string? CompanyName, string? Regon, string? Nip);

public abstract record ListClientsOrder(PaginationDto Pagination);
public record ListPersonClientsOrder(PaginationDto Pagination) : ListClientsOrder(Pagination);
public record ListCompanyClientsOrder(PaginationDto Pagination) : ListClientsOrder(Pagination);