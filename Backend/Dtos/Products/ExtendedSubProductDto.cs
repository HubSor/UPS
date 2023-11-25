using Dtos.Parameters;
using Models.Entities;

namespace Dtos.Products
{
	public class ExtendedSubProductDto : SubProductDto
	{
		public decimal Price { get; set; }
		public ICollection<ParameterDto> Parameters { get; set; } = default!;
		public ICollection<ProductDto> Products { get; set; } = default!;

		public ExtendedSubProductDto(SubProduct sp, decimal price) : base(sp)
		{
			Price = price;
			Parameters = sp.Parameters.Where(p => !p.Deleted).Select(p => new ParameterDto(p)).ToList();
		}

		public ExtendedSubProductDto(SubProduct sp) : base(sp)
		{
			Products = sp.SubProductInProducts
				.Where(x => !x.Product.Deleted)
				.Select(x => new ProductDto(x.Product))
				.ToList();

			Parameters = sp.Parameters.Where(p => !p.Deleted).Select(p => new ParameterDto(p)).ToList();
		}
	}
}
