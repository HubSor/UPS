namespace Messages.Products;

public record AddProductOrder(bool AnonymousSaleAllowed, string Code, string Name, 
	decimal BasePrice, string? Description);