using Dtos.Products;
using Models.Entities;

namespace Dtos.Sales
{
	public class SaleDetailsSubProductDto : SubProductDto
	{
		public decimal Price { get; set; }
		public decimal Tax { get; set; }
		
		public SaleDetailsSubProductDto(SubProduct sp, SubProductInSale sale) : base(sp)
		{
			Price = sale.Price;
			Tax = sale.Tax;
		}
	}
}
