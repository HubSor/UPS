using System.ComponentModel.DataAnnotations;
using Core.Dtos;
using Core.Models;

namespace UsersMicro.Models
{
	public abstract class Client : Entity<int>
	{
		public bool Deleted { get; set; }
		[MaxLength(15)]
		public string? PhoneNumber { get; set; }
		[MaxLength(256)]
		public string? Email { get; set; }
		public ICollection<ClientAddress> Addresses { get; set; } = default!;
	}

	public class CompanyClient : Client
	{
		[MaxLength(256)]
		public string CompanyName { get; set; } = default!;
		[MaxLength(14)]
		public string? Regon { get; set; }
		[MaxLength(10)]
		public string? Nip { get; set; }

		public CompanyClientDto ToDto()
		{
			return new CompanyClientDto()
			{
				PhoneNumber = PhoneNumber,
				Email = Email,
				Id = Id,
				Regon = Regon,
				CompanyName = CompanyName,
				Nip = Nip,
			};
		}
	}

	public class PersonClient : Client
	{
		[MaxLength(128)]
		public string FirstName { get; set; } = default!;
		[MaxLength(128)]
		public string LastName { get; set; } = default!;
		[MaxLength(11)]
		public string? Pesel { get; set; }
		
		public PersonClientDto ToDto()
		{
			return new PersonClientDto()
			{
				PhoneNumber = PhoneNumber,
				Email = Email,
				Id = Id,
				FirstName = FirstName,
				LastName = LastName,
				Pesel = Pesel,
			};
		}
	}
}