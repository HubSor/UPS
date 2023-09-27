using Dtos.Users;

namespace Messages.Users;

public class LoginResponse 
{
	public UserDto UserDto { get; set; } = default!;
}

public class LogoutResponse {}

public class AddUserResponse {}