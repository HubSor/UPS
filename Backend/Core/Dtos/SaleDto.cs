namespace Core.Dtos;

public class SaleDto
{
	public int SaleId { get; set; }
	public string ProductCode { get; set; } = default!;
	public ICollection<string> SubProductCodes { get; set; } = default!;
	public string ClientName { get; set; } = default!;
	public string SaleTime { get; set; } = default!;
	public decimal TotalPrice { get; set; }
}