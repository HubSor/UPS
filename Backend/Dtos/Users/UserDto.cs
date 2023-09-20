namespace Dtos.Users;
public class UserDto
{
    public string Username { get; set; } = default!;
    public ICollection<string> Roles { get; set; } = Array.Empty<string>();
}
