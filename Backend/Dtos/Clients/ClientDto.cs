using Models.Entities;

namespace Dtos.Clients
{
	public abstract class ClientDto
	{
		public string? PhoneNumber { get; set; }
		public string? Email { get; set; }
		public int Id { get; set; }
		public ClientDto(Client client)
		{
			PhoneNumber = client.PhoneNumber;
			Email = client.Email;
			Id = client.Id;
		}
	}
	
	public class CompanyClientDto : ClientDto 
	{
		public string CompanyName { get; set; } = default!;
		public string? Regon { get; set; }
		public string? Nip { get; set; }
		
		public CompanyClientDto(CompanyClient client) : base(client)
		{
			CompanyName = client.CompanyName;
			Regon = client.Regon;
			Nip = client.Nip;
		}
	}
	
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
