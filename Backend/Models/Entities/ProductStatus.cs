using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

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