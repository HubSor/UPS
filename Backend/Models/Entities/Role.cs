using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
	public enum RoleEnum
	{
		Administrator = 0,
		Seller = 1,
		UserManager = 2,
	}
	
	public class Role : DictEntity<RoleEnum>
	{
		[MaxLength(1000)]
		public string? Description { get; set; }
	}
}