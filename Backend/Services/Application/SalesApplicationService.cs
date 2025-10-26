using Core;
using Messages.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Helpers;
using Services.Domain;
using Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Dtos;
using Dtos.Users;
using Messages.Sales;
using Dtos.Sales;
using Dtos.Clients;
using System.Globalization;

namespace Services.Application;

public interface ISalesApplicationService : IBaseApplicationService
{
    Task<SaveSaleResponse> SaveSaleAsync(SaveSaleOrder order);
    Task<ListSalesResponse> ListSalesAsync(ListSalesOrder order);
    Task<GetSaleResponse> GetSaleAsync(GetSaleOrder order);
}

public class SalesApplicationService(
    ILogger<SalesApplicationService> logger,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor,
    IRepository<Sale> sales,
    IRepository<Parameter> parameters,
    IRepository<Product> products,
    IRepository<Client> clients
) : BaseApplicationService(logger, unitOfWork), ISalesApplicationService
{
    public async Task<GetSaleResponse> GetSaleAsync(GetSaleOrder order)
    {
		if (!await sales.GetAll().AnyAsync(x => x.Id == order.SaleId))
		{
			ThrowValidationException("SaleId", "Nie znaleziono transakcji");
		}
		
		var sale = await sales.GetAll()
			.Include(p => p.Client)
			.Include(x => x.Product)
			.Include(x => x.SubProducts)
			.ThenInclude(x => x.SubProduct)
			.Include(x => x.SaleParameters)
			.ThenInclude(x => x.Parameter)
			.ThenInclude(x => x.Options)
			.FirstAsync(p => p.Id == order.SaleId);

		var saleDetailsDto = new SaleDetailsDto(sale);
		logger.LogInformation("Got sale {SaleId} details", sale.Id);

		return new GetSaleResponse()
		{
			SaleDetailsDto = saleDetailsDto,
		};
    }

	public async Task<ListSalesResponse> ListSalesAsync(ListSalesOrder order)
	{
		var saleCount = sales.GetAll().Count();
		var saleList = await sales.GetAll()
			.OrderByDescending(x => x.Id)
			.Include(x => x.Client)
			.Include(x => x.Product)
			.Include(x => x.SubProducts)
			.ThenInclude(x => x.SubProduct)
			.Skip(order.Pagination.PageIndex * order.Pagination.PageSize)
			.Take(order.Pagination.PageSize)
			.Select(s => new SaleDto()
			{
				SaleId = s.Id,
				SaleTime = s.SaleTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
				ProductCode = s.Product.Code,
				SubProductCodes = s.SubProducts.Select(x => x.SubProduct.Code).ToList(),
				PersonClient = GetPersonClient(s.Client),
				CompanyClient = GetCompanyClient(s.Client),
				TotalPrice = s.FinalPrice,
			})
			.ToListAsync();
			
		logger.LogInformation("Listed sales");
		return new ListSalesResponse()
		{
			Sales = new PagedList<SaleDto>(
				saleList,
				saleCount,
				order.Pagination.PageIndex,
				order.Pagination.PageSize
			)
		};
	}
	
	private static PersonClientDto? GetPersonClient(Client? client) => client is PersonClient personClient ?
		new PersonClientDto(personClient) :
		null;

	private static CompanyClientDto? GetCompanyClient(Client? client) => client is CompanyClient companyClient ?
		new CompanyClientDto(companyClient) :
		null;

	public async Task<SaveSaleResponse> SaveSaleAsync(SaveSaleOrder order)
	{
		var product = await products.GetAll()
			.Include(x => x.SubProductInProducts)
			.ThenInclude(x => x.SubProduct)
			.ThenInclude(x => x.Parameters)
			.Include(x => x.Parameters)
			.FirstOrDefaultAsync(x => x.Id == order.ProductId && !x.Deleted);
		if (product == null)
		{
			ThrowValidationException("ProductId", "Nie znaleziono produktu");
		}
		
		if (product!.Status != ProductStatusEnum.Offered)
		{
			ThrowValidationException("ProductId", "Próba sprzedaży nieoferowanego produktu");
		}
		
		var subProducts = product.SubProductInProducts
			.Select(x => x.SubProduct);
		if (order.SubProducts.Any(sp => !subProducts.Select(x => x.Id).Contains(sp.SubProductId)))
		{
			ThrowValidationException("SubProductIds", "Nie znaleziono podproduktu");
		}
		
		if (order.ClientId.HasValue)
		{
			var client = await clients.GetAll().FirstOrDefaultAsync(c => c.Id == order.ClientId && !c.Deleted);
			if (client == null)
			{
				ThrowValidationException("ClientId", "Nie znaleziono klienta");
			}
		}
		
		var requiredParamsSubProduct = product.SubProductInProducts.SelectMany(x => x.SubProduct.Parameters.Where(s => s.Required));
		var requiredParamsProduct = product.Parameters.Where(p => p.Required);
		var requiredParams = requiredParamsSubProduct.Union(requiredParamsProduct);
		var respondedParamIds = order.Answers.Select(a => a.ParameterId).ToList();
		
		if (!requiredParams.All(rp => respondedParamIds.Contains(rp.Id) && !string.IsNullOrEmpty(order.Answers.First(x => x.ParameterId == rp.Id).Answer)))
		{
			ThrowValidationException("Answers", "Nie podano wszystkich wymaganych odpowiedzi");
		}

		var dbParams = await parameters.GetAll()
			.Include(p => p.Options)
			.Where(x => respondedParamIds.Contains(x.Id) && !x.Deleted)
			.ToListAsync();

		if (dbParams.Count != respondedParamIds.Count)
		{
			ThrowValidationException("Answers", "Nie znaleziono parametru");
		}
		
		var dbSubProductIds = subProducts.Select(x => x.Id).ToList();
		if (!dbParams.All(p => dbSubProductIds.Contains(p.SubProductId ?? -1) || p.ProductId == product.Id))
		{
			ThrowValidationException("Answers", "Próba zapisu parametru nieprzypisanego do produktu lub podproduktu");
		}

		foreach (var dbParam in dbParams)
		{
			var answer = order.Answers.FirstOrDefault(x => x.ParameterId == dbParam.Id);
			if (answer == null)
			{
				ThrowValidationException("Answers", "Nie znaleziono parametru");
			}

			if (dbParam.Required && string.IsNullOrEmpty(answer!.Answer))
			{
				ThrowValidationException("Answers", "Nie podano wszystkich wymaganych odpowiedzi");
			}

			if (!string.IsNullOrEmpty(answer!.Answer) && !ValidateAnswer(dbParam, answer.Answer))
			{
				ThrowValidationException("Answers", "Nie wszystkie odpowiedzi są poprawne");
			}
		}
		
		var totalPrice = order.ProductPrice + order.SubProducts.Select(s => s.Price).Sum();
		var newSale = new Sale
		{
			ClientId = order.ClientId,
			ProductId = order.ProductId,
			SellerId = httpContextAccessor.GetUserId(),
			FinalPrice = totalPrice,
			ProductPrice = order.ProductPrice,
			ProductTax = order.ProductPrice * product.TaxRate,
			SaleParameters = order.Answers
				.Where(a => !string.IsNullOrEmpty(a.Answer))
				.Select(a =>
				{
					var optionId = dbParams
						.First(rp => rp.Id == a.ParameterId).Options
						.FirstOrDefault(o => o.Value == a.Answer)?.Id;

					return new SaleParameter
					{
						Value = a.Answer,
						ParameterId = a.ParameterId,
						OptionId = optionId
					};
				}).ToList(),
			SubProducts = order.SubProducts.Select(sp => new SubProductInSale()
			{
				SubProductId = sp.SubProductId,
				Price = sp.Price,
				Tax = subProducts.First(x => x.Id == sp.SubProductId).TaxRate * sp.Price
			}).ToList(),
			SaleTime = DateTime.Now
		};
		
		await sales.AddAsync(newSale);

		logger.LogInformation("Saved new sale {SaleId}", newSale.Id);
		return new SaveSaleResponse();
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
}