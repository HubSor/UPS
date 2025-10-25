namespace Core.Dtos;

public class SaleDetailsDto 
{
	public int SaleId { get; set; }
	public string SaleTime { get; set; } = default!;
	public decimal TotalPrice { get; set; }
	public decimal TotalTax { get; set; }
	public decimal ProductPrice { get; set; }
	public decimal ProductTax { get; set; }
	public int? ClientId { get; set; }
	public string? ClientName { get; set; }
	public int ProductId { get; set; }
	public string ProductCode { get; set; } = default!;
	public string SubProductCodes { get; set; } = default!;
	public ICollection<SaleParameterDto> Parameters { get; set; } = [];
}