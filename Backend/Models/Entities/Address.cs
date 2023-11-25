using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{	
	public abstract class Address: Entity<int>
	{
		public AddressTypeEnum Type { get; set; }
		public AddressType TypeObject { get; set; } = default!;
		[MaxLength(128)]
		public string StreetNumber { get; set; } = default!;
		[MaxLength(128)]
		public string City { get; set; } = default!;
		[MaxLength(6)]
		public string PostalCode { get; set; } = default!;
		[MaxLength(128)]
		public string? Street { get; set; }
		[MaxLength(128)]
		public string? HouseNumber { get; set; }
	}

	public enum AddressTypeEnum
	{
		Residence = 0,
		Correspondence = 1,
		Registered = 2,
	}

	public class AddressType : DictEntity<AddressTypeEnum>
	{
		[MaxLength(64)]
		public string Name { get; set; } = default!;
	}

	public class ClientAddress : Address 
	{
		public int ClientId { get; set; }
		public virtual Client Client { get; set; } = default!;
	}
}
