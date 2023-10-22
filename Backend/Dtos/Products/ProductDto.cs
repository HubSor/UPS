using Models.Entities;

namespace Dtos.Products
{
	public class ProductDto 
	{
		public int Id { get; set; }
		public bool AnonymousSaleAllowed { get; set; }
		public string Name { get; set; } = default!;
		public string Code { get; set; } = default!;
		public decimal BasePrice { get; set; }
		public string? Description { get; set; }
		public ProductStatusEnum Status { get; set; }
	}
}
