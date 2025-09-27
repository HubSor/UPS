namespace Core.Dtos
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
}
