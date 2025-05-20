using Dtos;
using Models.Entities;

namespace Messages.Queries;

public record ListProductsQuery(ProductStatusEnum[] Statuses, PaginationDto Pagination) : Query;
public record ListSubProductsQuery(int? ProductId, PaginationDto Pagination) : Query;
public record GetProductQuery(int ProductId) : Query;
public record GetSubProductQuery(int SubProductId) : Query;