using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public class Sale : Entity<int>
	{
		[Column(TypeName = "money")]
		public decimal FinalPrice { get; set; }
		public int ProductId { get; set; }
		public virtual Product Product { get; set; } = default!;
		public int SellerId { get; set; }
		public virtual User Seller { get; set; } = default!;
		public IEnumerable<SubProductInSale> SubProducts { get; set; } = default!;
		public IEnumerable<SaleParameter> SaleParameters { get; set; } = default!;
	}
}