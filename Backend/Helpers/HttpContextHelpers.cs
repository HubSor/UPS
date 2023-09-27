using Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Helpers;
public static class HttpContextHelpers
{
	public static bool HasAnyRole(this IHttpContextAccessor context, params RoleEnum[] roles)
	{
		var roleClaim = context.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Role);
		return roleClaim != null && roles.Any(role => roleClaim.Value.Split(':').Any(x => x == role.ToString()));
	}
	
	public static bool IsAuthorized(this IHttpContextAccessor context)
	{
		var nameClaim = context.HttpContext.User.FindFirst(ClaimTypes.Name);
		return nameClaim?.Value != null && !string.IsNullOrEmpty(nameClaim.Value);
	}
}

