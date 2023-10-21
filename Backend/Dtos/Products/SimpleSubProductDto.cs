namespace Dtos.Products;

class SimpleSubProductDto 
{
	public string Name { get; set; } = default!;
	public string Code { get; set; } = default!;
	public decimal BasePrice { get; set; }
	public string? Description { get; set; }
}