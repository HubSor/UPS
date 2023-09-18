using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
	public enum Role
	{
		Administrator = 0,
		Seller = 1,
		UserManager = 2,
	}
	
	public class RoleEntity : Entity<Role>
	{
		[MaxLength(1000)]
		public string? Description { get; set; }
	}
}