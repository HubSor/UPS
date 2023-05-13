using Models.Dtos;

namespace Messages.Products;

public class AddProductResponse
{

}

public class ViewProductsResponse
{
    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
}
