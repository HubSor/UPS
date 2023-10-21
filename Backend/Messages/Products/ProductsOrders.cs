namespace Messages.Products;

public record AddProductOrder(bool AnonymousSaleAllowed, string Code, string Name, decimal BasePrice, string? Description);
public record AddSubProductOrder(string Code, string Name, decimal BasePrice, string? Description, int? ProductId);
public record AssignSubProductToProductOrder(int ProductId, int SubProductId, decimal Price);
public record UnAssignSubProductsFromProductOrder(int ProductId, int[] SubProductIds);