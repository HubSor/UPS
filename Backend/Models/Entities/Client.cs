using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
	public class Client : Entity<int>
	{
		public bool Deleted { get; set; }
		[MaxLength(15)]
		public string? PhoneNumber { get; set; }
		[MaxLength(256)]
		public string? Email { get; set; }
		public ICollection<ClientAddress> Addresses { get; set; } = default!;
	}
	
	public class FirmClient : Client
	{
		[MaxLength(256)]
		public string CompanyName { get; set; } = default!;
		[MaxLength(14)]
		public string? Regon { get; set; }
		[MaxLength(10)]
		public string? Nip { get; set; }
	}
	
	public class PersonClient: Client
	{
		[MaxLength(128)]
		public string FirstName { get; set; } = default!;
		[MaxLength(128)]
		public string LastName { get; set; } = default!;
		[MaxLength(11)]
		public string? Pesel { get; set; }
	}
}