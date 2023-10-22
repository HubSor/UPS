namespace Dtos.Products
{
	public class SubProductDto 
	{
		public int Id { get; set; }
		public string Name { get; set; } = default!;
		public string Code { get; set; } = default!;
		public decimal BasePrice { get; set; }
		public string? Description { get; set; }
	}
}
