using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Entities
{
	[Index(propertyName: nameof(Code), IsUnique = true)]
	public class Product : Entity<int>
	{
		public bool AnonymousSaleAllowed { get; set; }
		[MaxLength(6)]
		public string Code { get; set; } = default!;
		[MaxLength(128)]
		public string Name { get; set; } = default!;
		[Column(TypeName = "money")]
		public decimal Price { get; set; }
		public ProductStatusEnum Status { get; set; }
		[MaxLength(1000)]
		public string? Description { get; set; }
	}
}