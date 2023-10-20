using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public class SubProductInSale : IEntity
	{
		[Column(TypeName = "money")]
		public decimal InSalePrice { get; set; }
		public bool ManualOverride { get; set; }
		public int SaleId { get; set; }
		public virtual Sale Sale { get; set; } = default!;
		public int SubProductId { get; set; }
		public virtual SubProduct SubProduct { get; set; } = default!;
	}
}