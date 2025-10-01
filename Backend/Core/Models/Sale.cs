using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
	public class Sale : Entity<int>
	{
		[Column(TypeName = "money")]
		public decimal FinalPrice { get; set; }
		[Column(TypeName = "money")]
		public decimal ProductPrice { get; set; }
		[Column(TypeName = "money")]
		public decimal ProductTax { get; set; }
		public int ProductId { get; set; }
		public string ProductCode { get; set; } = default!;
		public string SubProductCodes { get; set; } = default!;
		public int SellerId { get; set; }
		public int? ClientId { get; set; }
		public string ClientName { get; set; } = default!;
		public DateTime SaleTime { get; set; }
	}
}