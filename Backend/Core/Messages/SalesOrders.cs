using Core.Dtos;

namespace Core.Messages;

public record SaveSaleOrder(int ProductId, int? ClientId, SaveSaleParameterDto[] Answers, decimal ProductPrice, SaveSaleSubProductDto[] SubProducts) : BaseOrder();
public record SaveSaleProductsMicroOrder(int SaleId, SaveSaleParameterDto[] Parameters, SaveSaleSubProductDto[] SubProducts) : BaseOrder();
public record ListSalesOrder(PaginationDto Pagination) : BaseOrder();
public record GetSaleOrder(int SaleId) : BaseOrder();