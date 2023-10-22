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
public class ListSubProductsResponse 
{
	public ICollection<SubProductDto> SubProducts { get; set; } = new List<SubProductDto>();
}
public class EditProductResponse {}
public class EditSubProductResponse {}
public class EditSubProductAssignmentResponse {}
public class DeleteProductResponse { }
public class DeleteSubProductResponse { }