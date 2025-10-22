using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Core.Web;
using Microsoft.EntityFrameworkCore;

namespace Core.Models
{
	[Index(nameof(Name), IsUnique = true)]
	public class User : Entity<int>
	{
		[MaxLength(128)]
		public string Name { get; set; } = default!;
		[MaxLength(64)]
		public byte[] Hash { get; set; } = default!;
		[MaxLength(32)]
		public byte[] Salt { get; set; } = default!;
		public bool Active { get; set; } = true;
		public ICollection<Role> Roles { get; set; } = default!;
		
		public ICollection<Claim> GetClaims()
		{
			var claims = new List<Claim>()
			{
				new (ClaimTypes.Name, Name),
				new (ClaimsHelpers.IdClaimType, Id.ToString())
			};
			claims.AddRange(Roles.Select(r => new Claim(ClaimTypes.Role, r.Id.ToString())));
			return claims;
		}
	}
}
