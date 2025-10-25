using Core.Dtos;

namespace Core.Messages;

public class LoginResponse 
{
	public UserDto UserDto { get; set; } = default!;
	public ICollection<ClaimDto> Claims { get; set; } = [];
}
public class AddUserResponse {}
public class ListUsersResponse 
{
	public PagedList<UserDto> Users { get; set; } = default!;
}
public class EditUserResponse
{
	public ICollection<ClaimDto> Claims { get; set; } = [];
}
public class DeleteUserResponse {}