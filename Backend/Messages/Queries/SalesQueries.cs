using Dtos;

namespace Messages.Queries;

public record ListSalesQuery(PaginationDto Pagination);
public record GetSaleQuery(int SaleId);