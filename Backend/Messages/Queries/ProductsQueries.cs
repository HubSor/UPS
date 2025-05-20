using Dtos;
using Models.Entities;

namespace Messages.Queries;

public record ListProductsQuery(ProductStatusEnum[] Statuses, PaginationDto Pagination);
public record ListSubProductsQuery(int? ProductId, PaginationDto Pagination);
public record GetProductQuery(int ProductId);
public record GetSubProductQuery(int SubProductId);