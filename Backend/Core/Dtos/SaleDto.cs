using Dtos.Clients;

namespace Core.Dtos;

public class SaleDto
{
	public int SaleId { get; set; }
	public string ProductCode { get; set; } = default!;
	public ICollection<string> SubProductCodes { get; set; } = default!;
	public PersonClientDto? PersonClient { get; set; } = default!; 
	public CompanyClientDto? CompanyClient { get; set; } = default!; 
	public string SaleTime { get; set; } = default!;
	public decimal TotalPrice { get; set; }
}