using Core.Models;

namespace Core.Dtos
{
	public class SaleDetailsSubProductDto : SubProductDto
	{
		public decimal Price { get; set; }
		public decimal Tax { get; set; }
		
		public SaleDetailsSubProductDto(){}
		public SaleDetailsSubProductDto(SubProduct sp, SubProductInSale sale) : base(sp)
		{
			Price = sale.Price;
			Tax = sale.Tax;
		}
	}
}
