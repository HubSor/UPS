using Core.Dtos;

namespace Core.Messages;

public class LoginResponse 
{
	public UserDto UserDto { get; set; } = default!;
}
public class LogoutResponse {}
public class AddUserResponse {}
public class ListUsersResponse 
{
	public PagedList<UserDto> Users { get; set; } = default!;
}
public class EditUserResponse {}
public class DeleteUserResponse {}