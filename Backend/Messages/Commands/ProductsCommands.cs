using Models.Entities;

namespace Messages.Commands;

public record AddProductOrder(bool AnonymousSaleAllowed, string Code, string Name, decimal BasePrice, int TaxRate, string? Description) : Command;
public record AddSubProductOrder(string Code, string Name, decimal BasePrice, string? Description, int TaxRate, int? ProductId) : Command;
public record AssignSubProductOrder(int ProductId, int SubProductId, decimal Price) : Command;
public record UnassignSubProductsOrder(int ProductId, int[] SubProductIds) : Command;
public record EditProductOrder(bool AnonymousSaleAllowed, string Code, string Name, decimal BasePrice, int TaxRate, string? Description, int Id, ProductStatusEnum Status) : Command;
public record EditSubProductOrder(string Code, string Name, decimal BasePrice, int TaxRate, string? Description, int Id) : Command;
public record EditSubProductAssignmentOrder(int ProductId, int SubProductId, decimal NewPrice) : Command;
public record DeleteProductOrder(int ProductId) : Command;
public record DeleteSubProductOrder(int SubProductId) : Command;