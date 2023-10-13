using Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Helpers;
public static class HttpContextHelpers
{
	public static bool HasAnyRole(this IHttpContextAccessor context, params RoleEnum[] roles)
	{
		var roleClaims = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role);
		return roles.Any(role => roleClaims.Any(rc => rc.Value == role.ToString()));
	}
	
	public static bool IsAuthorized(this IHttpContextAccessor context)
	{
		var nameClaim = context.HttpContext.User.FindFirst(ClaimTypes.Name);
		return nameClaim?.Value != null && !string.IsNullOrEmpty(nameClaim.Value);
	}
	
	public static int GetUserId(this IHttpContextAccessor context)
	{
		var idClaim = context.HttpContext.User.FindFirst(User.IdClaimType);
		return idClaim?.Value != null ? int.Parse(idClaim.Value) : 0;  
	}
}
