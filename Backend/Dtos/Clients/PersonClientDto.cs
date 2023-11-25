using Models.Entities;

namespace Dtos.Clients
{	
	public class PersonClientDto : ClientDto 
	{
		public string FirstName { get; set; } = default!;
		public string LastName { get; set; } = default!;
		public string? Pesel { get; set; }
		public PersonClientDto(PersonClient client) : base(client)
		{
			FirstName = client.FirstName;
			LastName = client.LastName;
			Pesel = client.Pesel;
		}
	}
}
