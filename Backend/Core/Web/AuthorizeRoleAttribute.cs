using Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace Core.Web;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
	public AuthorizeRolesAttribute(params RoleEnum[] roles): base()
	{
		Roles = string.Join(',', roles.Select(r => r.ToString()));
	}
}