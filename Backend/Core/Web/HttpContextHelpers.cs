using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Web;

public static class HttpContextHelpers
{
	public static bool HasAnyRole(this IHttpContextAccessor context, params RoleEnum[] roles)
	{
		var roleClaims = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role);
		return roles.Any(role => roleClaims.Any(rc => rc.Value == role.ToString()));
	}
	
	public static int GetUserId(this IHttpContextAccessor context)
	{
		var idClaim = context.HttpContext.User.FindFirst(User.IdClaimType);
		return idClaim?.Value != null ? int.Parse(idClaim.Value) : 0;  
	}
}
