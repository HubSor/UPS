using Core.Dtos;

namespace Core.Messages;

public abstract record FindClientOrder(string Identifier) : BaseOrder();
public record FindPersonClientOrder(string Identifier) : FindClientOrder(Identifier);
public record FindCompanyClientOrder(string Identifier) : FindClientOrder(Identifier);
public record UpsertClientOrder(bool IsCompany, int? ClientId, string? PhoneNumber, string? Email, string? FirstName,
	string? LastName, string? Pesel, string? CompanyName, string? Regon, string? Nip) : BaseOrder();

public abstract record ListClientsOrder(PaginationDto Pagination) : BaseOrder();
public record ListPersonClientsOrder(PaginationDto Pagination) : ListClientsOrder(Pagination);
public record ListCompanyClientsOrder(PaginationDto Pagination) : ListClientsOrder(Pagination);
public record GetClientOrder(int ClientId) : BaseOrder();