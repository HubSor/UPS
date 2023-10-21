using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Entities
{
	[Index(propertyName: nameof(Code), IsUnique = true)]
	public class SubProduct : Entity<int>
	{
		[MaxLength(128)]
		public string Name { get; set; } = default!;
		[MaxLength(6)]
		public string Code { get; set; } = default!;
		[Column(TypeName = "money")]
		public decimal BasePrice { get; set; }
		[MaxLength(1000)]
		public string? Description { get; set; }
		public IEnumerable<SubProductInProduct> SubProductInProducts { get; set; } = default!;
		public IEnumerable<Parameter> Parameters { get; set; } = default!;
	}
}