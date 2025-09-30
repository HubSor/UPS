using System.Globalization;
using Core;
using Core.Data;
using Core.Messages;
using Core.Models;
using Core.Web;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace SalesMicro.Consumers;

public class SaveSaleConsumer : TransactionConsumer<SaveSaleOrder, SaveSaleResponse>
{
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IRepository<Sale> sales;
	private readonly IRepository<Parameter> parameters;
	private readonly IRepository<Product> products;
	private readonly IRepository<Client> clients;
	private ICollection<Parameter> RelevantParameters { get; set; } = default!;
	private Product SelectedProduct { get; set; } = default!;
	private ICollection<SubProduct> SelectedSubProducts { get; set; } = default!;

	public SaveSaleConsumer(ILogger<SaveSaleConsumer> logger, IRepository<Sale> sales,
		IRepository<Parameter> parameters, IRepository<Product> products,
		IRepository<Client> clients, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
		: base(unitOfWork, logger)
	{
		this.sales = sales;
		this.products = products;
		this.parameters = parameters;
		this.clients = clients;
		this.httpContextAccessor = httpContextAccessor;
	}

	private static bool ValidateAnswer(Parameter parameter, string answer)
	{
		if (parameter.Type == ParameterTypeEnum.Integer)
		{
			if (!int.TryParse(answer, out _))
				return false;
		}

		if (parameter.Type == ParameterTypeEnum.Decimal)
		{
			if (!decimal.TryParse(answer, CultureInfo.InvariantCulture, out _))
				return false;
		}

		if (parameter.Type == ParameterTypeEnum.Select)
		{
			if (!parameter.Options.Select(o => o.Value).Contains(answer))
				return false;
		}

		if (parameter.Type == ParameterTypeEnum.Checkbox)
		{
			if (answer != "TAK" && answer != "NIE")
				return false;
		}

		return true;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<SaveSaleOrder> context)
	{
		var product = await products.GetAll()
			.Include(x => x.SubProductInProducts)
			.ThenInclude(x => x.SubProduct)
			.ThenInclude(x => x.Parameters)
			.Include(x => x.Parameters)
			.FirstOrDefaultAsync(x => x.Id == context.Message.ProductId && !x.Deleted);
		if (product == null)
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Nie znaleziono produktu");
			return false;
		}

		if (product.Status != ProductStatusEnum.Offered)
		{
			await RespondWithValidationFailAsync(context, "ProductId", "Próba sprzedaży nieoferowanego produktu");
			return false;
		}

		var subProducts = product.SubProductInProducts
			.Select(x => x.SubProduct);
		if (context.Message.SubProducts.Any(sp => !subProducts.Select(x => x.Id).Contains(sp.SubProductId)))
		{
			await RespondWithValidationFailAsync(context, "SubProductIds", "Nie znaleziono podproduktu");
			return false;
		}

		if (context.Message.ClientId.HasValue)
		{
			var client = await clients.GetAll().FirstOrDefaultAsync(c => c.Id == context.Message.ClientId && !c.Deleted);
			if (client == null)
			{
				await RespondWithValidationFailAsync(context, "ClientId", "Nie znaleziono klienta");
				return false;
			}
		}

		var requiredParamsSubProduct = product.SubProductInProducts.SelectMany(x => x.SubProduct.Parameters.Where(s => s.Required));
		var requiredParamsProduct = product.Parameters.Where(p => p.Required);
		var requiredParams = requiredParamsSubProduct.Union(requiredParamsProduct);
		var respondedParamIds = context.Message.Answers.Select(a => a.ParameterId).ToList();

		if (!requiredParams.All(rp => respondedParamIds.Contains(rp.Id) && !string.IsNullOrEmpty(context.Message.Answers.First(x => x.ParameterId == rp.Id).Answer)))
		{
			await RespondWithValidationFailAsync(context, "Answers", "Nie podano wszystkich wymaganych odpowiedzi");
			return false;
		}

		var dbParams = await parameters.GetAll()
			.Include(p => p.Options)
			.Where(x => respondedParamIds.Contains(x.Id) && !x.Deleted)
			.ToListAsync();

		if (dbParams.Count != respondedParamIds.Count)
		{
			await RespondWithValidationFailAsync(context, "Answers", "Nie znaleziono parametru");
			return false;
		}

		var dbSubProductIds = subProducts.Select(x => x.Id).ToList();
		if (!dbParams.All(p => dbSubProductIds.Contains(p.SubProductId ?? -1) || p.ProductId == product.Id))
		{
			await RespondWithValidationFailAsync(context, "Answers", "Próba zapisu parametru nieprzypisanego do produktu lub podproduktu");
			return false;
		}

		foreach (var dbParam in dbParams)
		{
			var answer = context.Message.Answers.FirstOrDefault(x => x.ParameterId == dbParam.Id);
			if (answer == null)
			{
				await RespondWithValidationFailAsync(context, "Answers", "Nie znaleziono parametru");
				return false;
			}

			if (dbParam.Required && string.IsNullOrEmpty(answer.Answer))
			{
				await RespondWithValidationFailAsync(context, "Answers", "Nie podano wszystkich wymaganych odpowiedzi");
				return false;
			}

			if (!string.IsNullOrEmpty(answer.Answer) && !ValidateAnswer(dbParam, answer.Answer))
			{
				await RespondWithValidationFailAsync(context, "Answers", "Nie wszystkie odpowiedzi są poprawne");
				return false;
			}
		}

		RelevantParameters = dbParams;
		SelectedProduct = product;
		SelectedSubProducts = subProducts.ToList();
		return true;
	}

	public override async Task InTransaction(ConsumeContext<SaveSaleOrder> context)
	{
		var totalPrice = context.Message.ProductPrice + context.Message.SubProducts.Select(s => s.Price).Sum();
		var newSale = new Sale
		{
			ClientId = context.Message.ClientId,
			ProductId = context.Message.ProductId,
			SellerId = httpContextAccessor.GetUserId(),
			FinalPrice = totalPrice,
			ProductPrice = context.Message.ProductPrice,
			ProductTax = context.Message.ProductPrice * SelectedProduct.TaxRate,
			SaleParameters = context.Message.Answers
				.Where(a => !string.IsNullOrEmpty(a.Answer))
				.Select(a =>
				{
					var optionId = RelevantParameters
						.First(rp => rp.Id == a.ParameterId).Options
						.FirstOrDefault(o => o.Value == a.Answer)?.Id;

					return new SaleParameter
					{
						Value = a.Answer,
						ParameterId = a.ParameterId,
						OptionId = optionId
					};
				}).ToList(),
			SubProducts = context.Message.SubProducts.Select(sp => new SubProductInSale()
			{
				SubProductId = sp.SubProductId,
				Price = sp.Price,
				Tax = SelectedSubProducts.First(x => x.Id == sp.SubProductId).TaxRate * sp.Price
			}).ToList(),
			SaleTime = DateTime.Now
		};

		await sales.AddAsync(newSale);

		logger.LogInformation("Saved new sale {SaleId}", newSale.Id);
	}

	public override async Task PostTransaction(ConsumeContext<SaveSaleOrder> context)
	{
		await RespondAsync(context, new SaveSaleResponse());
	}
}