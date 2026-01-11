using Dtos;
using Dtos.Sales;

namespace Messages.Sales;

public class SaveSaleResponse
{
	public int SaleId { get; set; }
}

public class ListSalesResponse 
{
	public PagedList<SaleDto> Sales { get; set; } = default!;
}
public class GetSaleResponse
{
	public SaleDetailsDto SaleDetailsDto { get; set; } = default!;
}