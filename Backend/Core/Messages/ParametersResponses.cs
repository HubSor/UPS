using Core.Dtos;

namespace Core.Messages;

public class AddParameterResponse {}
public class EditParameterResponse {}
public class DeleteParameterResponse {}
public class AddOptionResponse {}
public class DeleteOptionResponse { }
public class GetSaleParametersResponse
{
    public ProductDto Product { get; set; } = default!;
	public ICollection<SaleDetailsSubProductDto> SubProducts { get; set; } = default!;
	public ICollection<SaleParameterDto> Parameters { get; set; } = default!;
}