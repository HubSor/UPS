using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
	public class SubProductInSale : IEntity
	{
		public int SaleId { get; set; }
		public int SubProductId { get; set; }
		public virtual SubProduct SubProduct { get; set; } = default!;
		[Column(TypeName = "money")]
		public decimal Price { get; set; }
		[Column(TypeName = "money")]
		public decimal Tax { get; set; }
	}
}