using Dtos.Clients;

namespace Messages.Clients;

public class FindClientResponse 
{
	public ClientDto Client { get; set; } = default!;
}