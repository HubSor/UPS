using Dtos;

namespace Messages.Queries;

public record ListSalesQuery(PaginationDto Pagination) : Query;
public record GetSaleQuery(int SaleId) : Query;