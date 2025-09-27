using Core.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Web;

public static class HttpContextHelpers
{
	public const string IdClaimType = "Id"; 

	public static bool HasAnyRole(this IHttpContextAccessor context, params RoleEnum[] roles)
	{
		var roleClaims = context.HttpContext?.User.Claims.Where(x => x.Type == ClaimTypes.Role);
		if (roleClaims == null)
			return false;
		return roles.Any(role => roleClaims.Any(rc => rc.Value == role.ToString()));
	}
	
	public static int GetUserId(this IHttpContextAccessor context)
	{
		var idClaim = context.HttpContext?.User.FindFirst(IdClaimType);
		return idClaim?.Value != null ? int.Parse(idClaim.Value) : 0;  
	}
}
