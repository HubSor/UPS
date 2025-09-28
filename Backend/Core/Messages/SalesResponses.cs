using Core.Dtos;

namespace Core.Messages;

public class SaveSaleResponse {}
public class ListSalesResponse 
{
	public PagedList<SaleDto> Sales { get; set; } = default!;
}
public class GetSaleResponse
{
	public SaleDetailsDto SaleDetailsDto { get; set; } = default!;
}