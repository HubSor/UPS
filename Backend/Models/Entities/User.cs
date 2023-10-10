using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Models.Entities
{
	[Index(nameof(Name), IsUnique = true)]
	public class User : Entity<int>
	{
		[NotMapped]
		public static readonly string IdClaimType = "Id"; 
		
		public string Name { get; set; } = default!;
		[MaxLength(64)]
		public byte[] Hash { get; set; } = default!;
		[MaxLength(32)]
		public byte[] Salt { get; set; } = default!;
		public bool Active { get; set; } = true;
		public ICollection<Role> Roles { get; set; } = default!;
		
		public ICollection<Claim> GetClaims()
		{
			return new List<Claim>()
			{
				new (ClaimTypes.Name, Name),
				new (ClaimTypes.Role, string.Join(':', Roles.Select(x => x.Id.ToString()))),
				new (IdClaimType, Id.ToString())
			};
		}
	}
}
