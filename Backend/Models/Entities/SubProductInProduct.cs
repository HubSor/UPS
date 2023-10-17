using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public class SubProductInProduct
	{
		public int ProductId { get; set; }
		public virtual Product Product { get; set; } = default!;
		public int SubProductId { get; set; }
		public virtual SubProduct SubProduct { get; set; } = default!;
		[Column(TypeName = "money")]
		public decimal InProductPrice { get; set; }
	}
}