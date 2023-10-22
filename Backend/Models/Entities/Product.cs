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
		public decimal BasePrice { get; set; }
		public ProductStatusEnum Status { get; set; }
		public virtual ProductStatus StatusObject { get; set; } = default!;
		[MaxLength(1000)]
		public string? Description { get; set; }
		public ICollection<SubProductInProduct> SubProductInProducts { get; set; } = default!;
		public ICollection<Parameter> Parameters { get; set; } = default!;
		public ICollection<Sale> Sales { get; set; } = default!;
		public bool Deleted { get; set; }
	}
	
	public enum ProductStatusEnum 
	{
		NotOffered = 0,
		Offered = 1,
		Withdrawn = 2
	}
		
	public class ProductStatus : DictEntity<ProductStatusEnum>
	{
		[MaxLength(1000)]
		public string? Description { get; set; }
	}
}