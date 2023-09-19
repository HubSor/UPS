using Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Helpers;
public static class HttpContextHelpers
{
	public static bool HasAnyRole(this IHttpContextAccessor context, ICollection<Role> roles)
	{
		var roleClaim = context.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Role);
		return roleClaim != null && roles.Any(role => roleClaim.Value.Split(':').Any(x => x.Equals(role)));
	}
	
	public static bool HasRole(this IHttpContextAccessor context, Role role) => HasAnyRole(context, new List<Role>() { role });
	
	public static bool IsAuthorized(this IHttpContextAccessor context)
	{
		var nameClaim = context.HttpContext.User.FindFirst(ClaimTypes.Name);
		return nameClaim?.Value != null && !string.IsNullOrEmpty(nameClaim.Value);
	}
}

