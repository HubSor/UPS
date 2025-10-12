using Core.Dtos;

namespace Core.Messages;

public record SaveSaleOrder(int ProductId, int? ClientId, SaveSaleParameterDto[] Answers, decimal ProductPrice, SaveSaleSubProductDto[] SubProducts);
public record ExtendedSaveSaleOrder(
    int ProductId,
    int? ClientId,
    SaveSaleParameterDto[] Answers,
    decimal ProductPrice,
    SaveSaleSubProductDto[] SubProducts,
    ExtendedProductDto ProductDto,
    string ClientName
) : SaveSaleOrder(ProductId, ClientId, Answers, ProductPrice, SubProducts);

public record ListSalesOrder(PaginationDto Pagination);
public record GetSaleOrder(int SaleId);