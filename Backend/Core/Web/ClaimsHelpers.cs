using Core.Messages;
using Core.Models;
using System.Security.Claims;

namespace Core.Web;

public static class ClaimsHelpers
{
	public const string IdClaimType = "Id"; 

	public static bool HasAnyRole(this BaseOrder order, params RoleEnum[] roles)
	{
		var roleClaims = order.Claims.Where(x => x.Name == ClaimTypes.Role);
		if (roleClaims == null)
			return false;
		return roles.Any(role => roleClaims.Any(rc => rc.Value == role.ToString()));
	}
	
	public static int GetUserId(this BaseOrder order)
	{
		var idClaim = order.Claims.FirstOrDefault(c => c.Name == IdClaimType);
		return idClaim?.Value != null ? int.Parse(idClaim.Value) : 0;  
	}
}
