using Dtos.Clients;

namespace Dtos.Sales;
public class SaleDto
{
	public int SaleId { get; set; }
	public string ProductCode { get; set; } = default!;
	public ICollection<string> SubProductCodes { get; set; } = default!;
	public PersonClientDto? PersonClient { get; set; } = default!; 
	public CompanyClientDto? CompanyClient { get; set; } = default!; 
	public DateTime SaleTime { get; set; }
}