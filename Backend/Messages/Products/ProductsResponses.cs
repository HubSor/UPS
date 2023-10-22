using Dtos.Products;

namespace Messages.Products;

public class AddProductResponse {}
public class AddSubProductResponse {}
public class AssignSubProductResponse {}
public class UnassignSubProductsResponse {}
public class ListProductsResponse 
{
	public ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();
}