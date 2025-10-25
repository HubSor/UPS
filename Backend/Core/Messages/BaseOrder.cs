using System.Security.Claims;

namespace Core.Messages;

public abstract record BaseOrder
{
    public ICollection<ClaimDto>? Claims { get; set; } = [];
}

public class ClaimDto
{
    public string? Name { get; set; }
    public string? Value { get; set; }

    public Claim ToClaim()
    {
        return new Claim(Name!, Value!);
    }

    public ClaimDto() { }
    
    public ClaimDto(Claim claim)
    {
        Name = claim.Type;
        Value = claim.Value;
    }
}