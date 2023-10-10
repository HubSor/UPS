namespace Dtos.Users;
public class UserDto
{
	public int Id { get; set; }
	public string Username { get; set; } = default!;
	public ICollection<string> Roles { get; set; } = Array.Empty<string>();
}

public class EditUserDto : UserDto 
{
	public string Password { get; set; } = default!;
}