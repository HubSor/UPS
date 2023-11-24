using System.Globalization;
using Core;
using Data;
using Helpers;
using MassTransit;
using Messages.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Sales;
public class SaveSaleConsumer : TransactionConsumer<SaveSaleOrder, SaveSaleResponse>
{
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IRepository<Sale> sales;
	private readonly IRepository<Parameter> parameters;
	private readonly IRepository<Product> products;
	private readonly IRepository<Client> clients;
	private ICollection<Parameter> RelevantParameters { get; set; } = default!;
	
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

		return true;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<SaveSaleOrder> context)
	{
		var product = await products.GetAll()
			.Include(x => x.SubProductInProducts)
			.ThenInclude(x => x.SubProduct)
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
		
		var subProducts = product.SubProductInProducts.Select(x => x.SubProduct);
		if (context.Message.SubProductIds.Any(sp => !subProducts.Select(x => x.Id).Contains(sp)))
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
		
		var paramIds = context.Message.Answers.Select(a => a.ParameterId).ToList();
		var dbParams = await parameters.GetAll()
			.Include(p => p.Options)
			.Where(x => paramIds.Contains(x.Id) && !x.Deleted)
			.ToListAsync();
		if (dbParams.Count != paramIds.Count)
		{
			await RespondWithValidationFailAsync(context, "Answers", "Nie znaleziono parametru");
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
		return true;
	}

	public override async Task InTransaction(ConsumeContext<SaveSaleOrder> context)
	{
		await sales.AddAsync(new Sale
		{
			ClientId = context.Message.ClientId,
			ProductId = context.Message.ProductId,
			SellerId = httpContextAccessor.GetUserId(),
			FinalPrice = context.Message.TotalPrice,
			SaleParameters = context.Message.Answers
				.Where(a => !string.IsNullOrEmpty(a.Answer))
				.Select(a => {
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
			SaleTime = DateTime.Now
		});
	}

	public override async Task PostTransaction(ConsumeContext<SaveSaleOrder> context)
	{
		await RespondAsync(context, new SaveSaleResponse());
	}
}