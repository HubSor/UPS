using Core;
using Core.Data;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicro.Consumers;

public class SaveSaleConsumer(
	ILogger<SaveSaleConsumer> _logger,
	IRepository<SubProduct> subProductsRepo,
	IRepository<SubProductInSale> subProductsInSaleRepo,
	IRepository<SaleParameter> saleParametersRepo,
	IRepository<Parameter> parametersRepo,
	IUnitOfWork unitOfWork
) : TransactionConsumer<SaveSaleProductsMicroOrder, SaveSaleResponse>(unitOfWork, _logger)
{
	public override async Task InTransaction(ConsumeContext<SaveSaleProductsMicroOrder> context)
	{
		var subProductIds = context.Message.SubProducts.Select(x => x.SubProductId);
		var subProducts = await subProductsRepo.GetAll()
			.Where(x => subProductIds.Contains(x.Id))
			.ToListAsync();

		foreach (var selectedSubProduct in context.Message.SubProducts)
		{
			await subProductsInSaleRepo.AddAsync(new SubProductInSale()
			{
				SaleId = context.Message.SaleId,
				SubProductId = selectedSubProduct.SubProductId,
				Price = selectedSubProduct.Price,
				Tax = subProducts.First(s => s.Id == selectedSubProduct.SubProductId).TaxRate * selectedSubProduct.Price,
			});
		}

		var parameterIds = context.Message.Parameters.Select(p => p.ParameterId);
		var parameters = await parametersRepo.GetAll()
			.Where(p => parameterIds.Contains(p.Id))
			.Include(x => x.Options)
			.ToListAsync();

		foreach (var saleParameter in context.Message.Parameters)
        {
			var parameter = parameters.First(p => p.Id == saleParameter.ParameterId);

			await saleParametersRepo.AddAsync(new SaleParameter()
			{
				SaleId = context.Message.SaleId,
				ParameterId = parameter.Id,
				Value = saleParameter.Answer,
				OptionId = parameter.Options.FirstOrDefault(o => o.Value == saleParameter.Answer)?.Id,
			});
        }
	}

    public override Task PostTransaction(ConsumeContext<SaveSaleProductsMicroOrder> context)
    {
		return RespondAsync(context, new SaveSaleResponse(){ 
			SaleId = context.Message.SaleId
		});
    }
}
