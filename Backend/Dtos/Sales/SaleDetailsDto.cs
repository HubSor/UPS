using Dtos.Clients;
using Dtos.Products;
using Models.Entities;

namespace Dtos.Sales;
public class SaleDetailsDto 
{
	public SaleDetailsDto(Sale sale)
	{
		SaleId = sale.Id;
		SaleTime = sale.SaleTime.ToString("dd/MM/yyyy HH:mm");
		TotalPrice = sale.FinalPrice;
		PersonClient = sale.Client is PersonClient personClient ?
			new PersonClientDto(personClient) :
			null;
		CompanyClient = sale.Client is CompanyClient companyClient ?
			new CompanyClientDto(companyClient) :
			null;
		Product = new ProductDto(sale.Product);
		SubProducts = sale.SubProducts.Select(x => new SubProductDto(x.SubProduct)).ToList();
		Parameters = sale.SaleParameters.OrderBy(x => x.Parameter.Id).Select(sp => new SaleParameterDto(sp.Parameter, sp.Value)).ToList();
	}

	public int SaleId { get; set; }
	public string SaleTime { get; set; } = default!;
	public decimal TotalPrice { get; set; }
	public PersonClientDto? PersonClient { get; set; } = default!;
	public CompanyClientDto? CompanyClient { get; set; } = default!;
	public ProductDto Product { get; set; } = default!;
	public ICollection<SubProductDto> SubProducts { get; set; } = default!;
	public ICollection<SaleParameterDto> Parameters { get; set; } = default!;
}