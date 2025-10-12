namespace Core.Dtos
{	
	public class PersonClientDto : ClientDto 
	{
		public string FirstName { get; set; } = default!;
		public string LastName { get; set; } = default!;
		public string? Pesel { get; set; }

		public string GetName() => FirstName + " " + LastName;
	}
}
