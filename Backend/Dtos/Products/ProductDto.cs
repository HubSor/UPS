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
		
		public ProductDto(Product p)
		{
			Id = p.Id;
			Name = p.Name;
			Description = p.Description;
			Code = p.Code;
			Status = p.Status;
			AnonymousSaleAllowed = p.AnonymousSaleAllowed;
			BasePrice = p.BasePrice;
		}
	}
	
	public class ExtendedProductDto : ProductDto
	{
		public ICollection<ExtendedSubProductDto> SubProducts { get; set; } = default!;
		
		public ExtendedProductDto(Product p) : base(p)
		{
			SubProducts = p.SubProductInProducts
				.Select(x => new ExtendedSubProductDto(x.SubProduct, x.InProductPrice))
				.ToList();
		}
	}
}
