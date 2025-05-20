using Dtos.Sales;

namespace Messages.Commands;

public record SaveSaleOrder(int ProductId, int? ClientId, SaveSaleParameterDto[] Answers, decimal ProductPrice, SaveSaleSubProductDto[] SubProducts) : Command;