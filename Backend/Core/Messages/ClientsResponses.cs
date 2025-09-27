using Core.Dtos;

namespace Core.Messages;

public abstract class FindClientResponse { }

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

public class ListCompanyClientsResponse
{
	public PagedList<CompanyClientDto> Clients { get; set; } = default!;
}

public class ListPersonClientsResponse 
{
	public PagedList<PersonClientDto> Clients { get; set; } = default!;
}