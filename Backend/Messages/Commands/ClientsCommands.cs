namespace Messages.Commands;

public record UpsertClientOrder(bool IsCompany, int? ClientId, string? PhoneNumber, string? Email, string? FirstName,
	string? LastName, string? Pesel, string? CompanyName, string? Regon, string? Nip) : Command;
