using Core.Models;

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
}