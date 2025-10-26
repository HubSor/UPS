using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Data;
using Dtos;
using Dtos.Clients;
using Messages.Clients;

namespace Services.Application;

public interface IClientsApplicationService : IBaseApplicationService
{
    Task<UpsertClientResponse> UpsertClientAsync(UpsertClientOrder order);
    Task<FindPersonClientResponse> FindPersonAsync(FindPersonClientOrder order);
    Task<ListPersonClientsResponse> ListPeopleAsync(ListPersonClientsOrder order);
    Task<FindCompanyClientResponse> FindCompanyAsync(FindCompanyClientOrder order);
    Task<ListCompanyClientsResponse> ListCompaniesAsync(ListCompanyClientsOrder order);
}

public class ClientsApplicationService(
    ILogger<ClientsApplicationService> logger,
    IUnitOfWork unitOfWork,
    IRepository<Client> clients,
    IRepository<PersonClient> personClients,
    IRepository<CompanyClient> companyClients
) : BaseApplicationService(logger, unitOfWork), IClientsApplicationService
{
    public async Task<FindCompanyClientResponse> FindCompanyAsync(FindCompanyClientOrder order)
	{
		var foundClient = await companyClients.GetAll()
			.FirstOrDefaultAsync(x => !x.Deleted && (x.Nip == order.Identifier.ToUpper().Trim() ||
				x.Regon == order.Identifier.ToUpper().Trim()));
		if (foundClient == null)
		{
			ThrowValidationException("Identifier", "Nie znaleziono klienta");
		}

		return new FindCompanyClientResponse()
		{
			CompanyClient = new CompanyClientDto(foundClient!),
		};
    }

    public async Task<FindPersonClientResponse> FindPersonAsync(FindPersonClientOrder order)
    {
		var foundClient = await personClients.GetAll()
			.FirstOrDefaultAsync(x => x.Pesel == order.Identifier.ToUpper().Trim() && !x.Deleted);
		if (foundClient == null)
		{
			ThrowValidationException("Identifier", "Nie znaleziono klienta");
		}

		return new FindPersonClientResponse()
		{
			PersonClient = new PersonClientDto(foundClient!),
		};
    }

    public async Task<ListCompanyClientsResponse> ListCompaniesAsync(ListCompanyClientsOrder order)
    {
        var count = companyClients.GetAll().Count();
		var list = companyClients.GetAll()
			.OrderByDescending(x => x.Id)
			.Skip(order.Pagination.PageIndex * order.Pagination.PageSize)
			.Take(order.Pagination.PageSize)
			.Select(x => new CompanyClientDto(x))
			.ToList();

		logger.LogInformation("Listing clients of type {ClientType}", nameof(CompanyClient));
		return new ListCompanyClientsResponse()
		{
			Clients = new PagedList<CompanyClientDto>(
				list,
				count,
				order.Pagination.PageIndex,
				order.Pagination.PageSize
			)
		};
    }

    public async Task<ListPersonClientsResponse> ListPeopleAsync(ListPersonClientsOrder order)
    {
		var count = personClients.GetAll().Count();
		var list = personClients.GetAll()
			.OrderByDescending(x => x.Id)
			.Skip(order.Pagination.PageIndex * order.Pagination.PageSize)
			.Take(order.Pagination.PageSize)
			.Select(x => new PersonClientDto(x))
			.ToList();

		logger.LogInformation("Listing clients of type {ClientType}", nameof(PersonClient));
		return new ListPersonClientsResponse()
		{
			Clients = new PagedList<PersonClientDto>(
				list,
				count,
				order.Pagination.PageIndex,
				order.Pagination.PageSize
			)
		};
    }

    public async Task<UpsertClientResponse> UpsertClientAsync(UpsertClientOrder order)
	{
		var clientFound = false;
		Client? client = null;
        if (order.ClientId.HasValue)
		{
			client = await clients.GetAll().FirstOrDefaultAsync(x => x.Id == order.ClientId && !x.Deleted);
			if (client != null && ((client is PersonClient && order.IsCompany) || (client is CompanyClient && !order.IsCompany)))
			{
				ThrowValidationException("ClientId", "Niedozwolona próba zmiany typu klienta");
			}
			if (client != null)
            {
				clientFound = true;
            }
		}

		if (client == null)
			logger.LogInformation("Client not found. Insert mode");
		else
			logger.LogInformation("Client {ClientId} found. Update mode", client.Id);
			
		client ??= order.IsCompany ? new CompanyClient() : new PersonClient();
		
		client.PhoneNumber = order.PhoneNumber;
		client.Email = order.Email;
		if (client is PersonClient personClient)
		{
			personClient.Pesel = order.Pesel;
			personClient.FirstName = order.FirstName!;
			personClient.LastName = order.LastName!;
			logger.LogInformation("Upserting person");
		}
		if (client is CompanyClient companyClient)
		{
			companyClient.CompanyName = order.CompanyName!;
			companyClient.Nip = order.Nip;
			companyClient.Regon = order.Regon;
			logger.LogInformation("Upserting company");
		}

		if (clientFound)
			await clients.UpdateAsync(client);
		else
			await clients.AddAsync(client);

		return new UpsertClientResponse()
		{
			ClientId = client.Id
		};
    }
}