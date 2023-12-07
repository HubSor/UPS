using Dtos;
using Dtos.Sales;

namespace Messages.Sales;

public record SaveSaleOrder(int ProductId, int? ClientId, SaveSaleParameterDto[] Answers, decimal ProductPrice, SaveSaleSubProductDto[] SubProducts);
public record ListSalesOrder(PaginationDto Pagination);
public record GetSaleOrder(int SaleId);