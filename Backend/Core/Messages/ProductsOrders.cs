using Core.Dtos;
using Core.Models;

namespace Core.Messages;

public record AddProductOrder(bool AnonymousSaleAllowed, string Code, string Name, decimal BasePrice, int TaxRate, string? Description);
public record AddSubProductOrder(string Code, string Name, decimal BasePrice, string? Description, int TaxRate, int? ProductId);
public record AssignSubProductOrder(int ProductId, int SubProductId, decimal Price);
public record UnassignSubProductsOrder(int ProductId, int[] SubProductIds);
public record ListProductsOrder(ProductStatusEnum[] Statuses, PaginationDto Pagination);
public record ListSubProductsOrder(int? ProductId, PaginationDto Pagination);
public record EditProductOrder(bool AnonymousSaleAllowed, string Code, string Name, decimal BasePrice, int TaxRate, string? Description, int Id, ProductStatusEnum Status);
public record EditSubProductOrder(string Code, string Name, decimal BasePrice, int TaxRate, string? Description, int Id);
public record EditSubProductAssignmentOrder(int ProductId, int SubProductId, decimal NewPrice);
public record DeleteProductOrder(int ProductId);
public record DeleteSubProductOrder(int SubProductId);
public record GetProductOrder(int ProductId);
public record GetSubProductOrder(int SubProductId);