using Dtos.Products;
using Models.Entities;

namespace Messages.Products;

public record AddProductOrder(bool AnonymousSaleAllowed, string Code, string Name, decimal BasePrice, string? Description);
public record AddSubProductOrder(string Code, string Name, decimal BasePrice, string? Description, int? ProductId);
public record AssignSubProductOrder(int ProductId, int SubProductId, decimal Price);
public record UnassignSubProductsOrder(int ProductId, int[] SubProductIds);
public record ListProductsOrder(ProductStatusEnum[] Statuses);
public record ListSubProductsOrder(int? ProductId);
public record EditProductOrder(ProductDto ProductDto);
public record EditSubProductOrder(SubProductDto SubProductDto);