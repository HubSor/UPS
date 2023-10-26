using Dtos;
using Dtos.Products;

namespace Messages.Products;

public class AddProductResponse {}
public class AddSubProductResponse {}
public class AssignSubProductResponse {}
public class UnassignSubProductsResponse {}
public class ListProductsResponse 
{
	public PagedList<ProductDto> Products { get; set; } = default!;
}
public class ListSubProductsResponse 
{
	public PagedList<SubProductDto> SubProducts { get; set; } = default!;
}
public class EditProductResponse {}
public class EditSubProductResponse {}
public class EditSubProductAssignmentResponse {}
public class DeleteProductResponse { }
public class DeleteSubProductResponse { }