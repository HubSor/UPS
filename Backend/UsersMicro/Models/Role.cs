using System.ComponentModel.DataAnnotations;
using Data;

namespace UsersMicro.Models
{
	public enum RoleEnum
	{
		Administrator = 0,
		Seller = 1,
		UserManager = 2,
		ProductManager = 3,
		SaleManager = 4,
		ClientManager = 5,
	}
	
	public class Role : DictEntity<RoleEnum>
	{
		[MaxLength(64)]
		public string Name { get; set; } = default!;
		[MaxLength(1000)]
		public string? Description { get; set; }
	}
}