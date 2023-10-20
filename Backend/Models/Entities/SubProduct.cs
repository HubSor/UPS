using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public class SubProduct : Entity<int>
	{
		[MaxLength(128)]
		public string Name { get; set; } = default!;
		[Column(TypeName = "money")]
		public decimal BasePrice { get; set; }
		[MaxLength(1000)]
		public string? Description { get; set; }
		public IEnumerable<SubProductInProduct> SubProductInProducts { get; set; } = default!;
		public IEnumerable<Parameter> Parameters { get; set; } = default!;
	}
}