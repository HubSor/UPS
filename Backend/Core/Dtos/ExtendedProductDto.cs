using Core.Models;

namespace Core.Dtos
{
	public class ExtendedProductDto : ProductDto
	{
		public ICollection<ExtendedSubProductDto> SubProducts { get; set; } = default!;
		public ICollection<ParameterDto> Parameters { get; set; } = default!;

		public ExtendedProductDto(){}
		public ExtendedProductDto(Product p) : base(p)
		{
			SubProducts = p.SubProductInProducts
				.Where(x => !x.SubProduct.Deleted)
				.Select(x => new ExtendedSubProductDto(x.SubProduct, x.InProductPrice))
				.ToList();
				
			Parameters = p.Parameters.Where(p => !p.Deleted).Select(p => new ParameterDto(p)).ToList();
		}
	}
}
