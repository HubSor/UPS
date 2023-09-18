using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Models.Entities
{
	[Index(nameof(Name), IsUnique = true)]
	public class User : Entity<int>
	{
		public string Name { get; set; } = default!;
		[MaxLength(64)]
		public byte[] Hash { get; set; } = default!;
		[MaxLength(32)]
		public byte[] Salt { get; set; } = default!;
		public bool Active { get; set; } = true;
		public ICollection<RoleEntity> Roles { get; set; } = default!;
		
		public ICollection<Claim> GetClaims()
		{
			return new List<Claim>()
			{
				new Claim(ClaimTypes.Name, Name),
				new Claim(ClaimTypes.Role, string.Join(':', Roles.Select(x => x.ToString()))),
			};
		}
	}
}
