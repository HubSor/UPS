using Models.Entities;

namespace Dtos.Clients
{
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
}
