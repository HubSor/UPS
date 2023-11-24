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
		public int? ClientId { get; set; }
		public virtual Client? Client { get; set; } = default!;
		public ICollection<SubProductInSale> SubProducts { get; set; } = default!;
		public ICollection<SaleParameter> SaleParameters { get; set; } = default!;
		public DateTime SaleTime { get; set; }
	}
}