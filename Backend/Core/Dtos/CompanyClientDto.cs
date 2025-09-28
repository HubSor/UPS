namespace Core.Dtos
{
	public class CompanyClientDto : ClientDto 
	{
		public string CompanyName { get; set; } = default!;
		public string? Regon { get; set; }
		public string? Nip { get; set; }
	}
}
