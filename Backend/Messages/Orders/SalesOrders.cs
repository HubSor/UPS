using Dtos.Sales;

namespace Messages.Orders;

public record SaveSaleOrder(int ProductId, int? ClientId, SaveSaleParameterDto[] Answers, decimal ProductPrice, SaveSaleSubProductDto[] SubProducts);