using Data;

namespace UsersMicro.Models;

public class ClientAddress : Address
{
    public int ClientId { get; set; }
    public virtual Client Client { get; set; } = default!;
}