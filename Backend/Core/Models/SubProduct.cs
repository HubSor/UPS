using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Models
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
		[Column(TypeName = "numeric(3, 2)")]
		public decimal TaxRate { get; set; }
		[MaxLength(1000)]
		public string? Description { get; set; }
		public ICollection<SubProductInProduct> SubProductInProducts { get; set; } = default!;
		public ICollection<SubProductInSale> SubProductInSales { get; set; } = default!;
		public ICollection<Parameter> Parameters { get; set; } = default!;
		public bool Deleted { get; set; }
	}
}