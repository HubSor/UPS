using Dtos.Clients;

namespace Messages.Clients;

public abstract class FindClientResponse {}
public class FindPersonClientResponse : FindClientResponse 
{
	public PersonClientDto PersonClient { get; set; } = default!;
}
public class FindCompanyClientResponse : FindClientResponse
{
	public CompanyClientDto CompanyClient { get; set; } = default!;
}
public class UpsertClientResponse 
{
	public int ClientId { get; set; }
}