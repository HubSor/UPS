namespace UsersMicro.Dtos;

public class UserDto
{
	public int Id { get; set; }
	public string Username { get; set; } = default!;
	public ICollection<string> Roles { get; set; } = Array.Empty<string>();
}