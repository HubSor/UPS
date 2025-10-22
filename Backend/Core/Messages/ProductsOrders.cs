using Core.Dtos;
using Core.Models;

namespace Core.Messages;

public record AddProductOrder(bool AnonymousSaleAllowed, string Code, string Name, decimal BasePrice, int TaxRate, string? Description) : BaseOrder();
public record AddSubProductOrder(string Code, string Name, decimal BasePrice, string? Description, int TaxRate, int? ProductId) : BaseOrder();
public record AssignSubProductOrder(int ProductId, int SubProductId, decimal Price) : BaseOrder();
public record UnassignSubProductsOrder(int ProductId, int[] SubProductIds) : BaseOrder();
public record ListProductsOrder(ProductStatusEnum[] Statuses, PaginationDto Pagination) : BaseOrder();
public record ListSubProductsOrder(int? ProductId, PaginationDto Pagination) : BaseOrder();
public record EditProductOrder(bool AnonymousSaleAllowed, string Code, string Name, decimal BasePrice, int TaxRate, string? Description, int Id, ProductStatusEnum Status) : BaseOrder();
public record EditSubProductOrder(string Code, string Name, decimal BasePrice, int TaxRate, string? Description, int Id) : BaseOrder();
public record EditSubProductAssignmentOrder(int ProductId, int SubProductId, decimal NewPrice) : BaseOrder();
public record DeleteProductOrder(int ProductId) : BaseOrder();
public record DeleteSubProductOrder(int SubProductId) : BaseOrder();
public record GetProductOrder(int ProductId) : BaseOrder();
public record GetSubProductOrder(int SubProductId) : BaseOrder();