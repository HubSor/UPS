using Dtos;
using Dtos.Sales;

namespace Messages.Sales;

public record SaveSaleOrder(int ProductId, int[] SubProductIds, int? ClientId, SaveSaleParameterDto[] Answers, decimal TotalPrice);
public record ListSalesOrder(PaginationDto Pagination);