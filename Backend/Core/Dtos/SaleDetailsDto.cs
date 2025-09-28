using Dtos.Products;

namespace Core.Dtos;

public class SaleDetailsDto 
{
	public int SaleId { get; set; }
	public string SaleTime { get; set; } = default!;
	public decimal TotalPrice { get; set; }
	public decimal TotalTax { get; set; }
	public decimal ProductPrice { get; set; }
	public decimal ProductTax { get; set; }
	public PersonClientDto? PersonClient { get; set; } = default!;
	public CompanyClientDto? CompanyClient { get; set; } = default!;
	public ProductDto Product { get; set; } = default!;
	public ICollection<SaleDetailsSubProductDto> SubProducts { get; set; } = default!;
	public ICollection<SaleParameterDto> Parameters { get; set; } = default!;
	
	public SaleDetailsDto(Sale sale)
	{
		SaleId = sale.Id;
		SaleTime = sale.SaleTime.ToString("dd/MM/yyyy HH:mm");
		TotalPrice = sale.FinalPrice;
		ProductPrice = sale.ProductPrice;
		ProductTax = sale.ProductTax;
		PersonClient = sale.Client is PersonClient personClient ?
			new PersonClientDto(personClient) :
			null;
		CompanyClient = sale.Client is CompanyClient companyClient ?
			new CompanyClientDto(companyClient) :
			null;
		Product = new ProductDto(sale.Product);
		SubProducts = sale.SubProducts.Select(x => new SaleDetailsSubProductDto(x.SubProduct, x)).ToList();
		Parameters = sale.SaleParameters.OrderBy(x => x.Parameter.Id).Select(sp => new SaleParameterDto(sp.Parameter, sp.Value)).ToList();
		TotalTax = sale.ProductTax + sale.SubProducts.Select(x => x.Tax).Sum();
	}
}