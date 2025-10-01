using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicro.Consumers;

public class GetSaleParametersConsumer(
	ILogger<GetSaleParametersConsumer> _logger,
	IRepository<Product> productsRepo,
	IRepository<SubProductInSale> subProductsRepo,
	IRepository<SaleParameter> saleParametersRepo,
	IUnitOfWork unitOfWork
) : TransactionConsumer<GetSaleParametersOrder, GetSaleParametersResponse>(unitOfWork, _logger)
{
	private GetSaleParametersResponse _resp = default!;

	public override async Task InTransaction(ConsumeContext<GetSaleParametersOrder> context)
	{
		var product = await productsRepo.GetAll()
			.FirstAsync(x => x.Id == context.Message.ProductId);

		var productDto = new ProductDto(product);

		var subProducts = await subProductsRepo.GetAll()
			.Where(x => x.SaleId == context.Message.SaleId)
			.Include(x => x.SubProduct)
			.ToListAsync();

		var subProductDtos = subProducts.Select(x => new SaleDetailsSubProductDto(x.SubProduct, x));

		var parameters = await saleParametersRepo.GetAll()
			.Where(x => x.SaleId == context.Message.SaleId)
			.Include(x => x.Parameter)
			.ToListAsync();

		var parameterDtos = parameters.Select(x => new SaleParameterDto(x.Parameter, x.Value));

		_resp = new GetSaleParametersResponse()
		{
			SubProducts = [.. subProductDtos],
			Parameters = [.. parameterDtos],
			Product = productDto,
		};
	}

    public override Task PostTransaction(ConsumeContext<GetSaleParametersOrder> context)
    {
		return RespondAsync(context, _resp);
    }
}
