using Microsoft.AspNetCore.Authorization;
using Models.Entities;

namespace UPS.Attributes;

public class AuthorizeRolesAttribute: AuthorizeAttribute
{
	public AuthorizeRolesAttribute(params RoleEnum[] roles): base()
	{
		Roles = string.Join(',', roles.Select(r => r.ToString()));
	}
}