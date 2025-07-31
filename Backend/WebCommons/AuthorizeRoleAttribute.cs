using Microsoft.AspNetCore.Authorization;
using Models.Entities;

namespace WebCommons;

public class AuthorizeRolesAttribute: AuthorizeAttribute
{
	public AuthorizeRolesAttribute(params RoleEnum[] roles): base()
	{
		Roles = string.Join(',', roles.Select(r => r.ToString()));
	}
}