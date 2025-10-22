namespace Core.Messages;

public abstract record BaseOrder
{
    public ICollection<ClaimDto> Claims { get; set; } = [];
}

public class ClaimDto
{
    public string? Name { get; set; }
    public string? Value { get; set; }
}