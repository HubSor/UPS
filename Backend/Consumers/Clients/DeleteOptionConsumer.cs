using Core;
using Data;
using Dtos.Clients;
using MassTransit;
using Messages.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Clients;
public class FindClientConsumer : TransactionConsumer<FindClientOrder, FindClientResponse>
{
	private readonly IRepository<CompanyClient> companyClients;
	private readonly IRepository<PersonClient> personClients;
	private Client client = default!;
	private ClientDto clientDto = default!;
	
	public FindClientConsumer(ILogger<FindClientConsumer> logger, IRepository<PersonClient> personClients,
		IRepository<CompanyClient> companyClients, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.personClients = personClients;
		this.companyClients = companyClients;
	}
	
	private async Task<PersonClient?> GetPersonClientAsync(ConsumeContext<FindClientOrder> context)
	{
		return await personClients.GetAll()
			.FirstOrDefaultAsync(x => x.Pesel == context.Message.Identifier.ToUpper().Trim());
	}

	private async Task<CompanyClient?> GetPCompanyClientAsync(ConsumeContext<FindClientOrder> context)
	{
		return await companyClients.GetAll()
			.FirstOrDefaultAsync(x => x.Nip == context.Message.Identifier.ToUpper().Trim() ||
				x.Regon == context.Message.Identifier.ToUpper().Trim());
	}

	public override async Task<bool> PreTransaction(ConsumeContext<FindClientOrder> context)
	{
		var foundClient = context.Message.IsPerson ? await GetPersonClientAsync(context) : await GetPCompanyClientAsync(context) as Client;

		if (foundClient == null)
		{
			await RespondWithValidationFailAsync(context, "Identifier", "Nie znaleziono klienta");
			return false;
		}

		client = foundClient;

		return true;
	}

	public override Task InTransaction(ConsumeContext<FindClientOrder> context)
	{
		clientDto = client is PersonClient personClient ? new PersonClientDto(personClient) :
			client is CompanyClient companyClient ? new CompanyClientDto(companyClient) :
				throw new UPSException("Illegal client object");

		return Task.CompletedTask;
	}

	public override async Task PostTransaction(ConsumeContext<FindClientOrder> context)
	{
		await RespondAsync(context, new FindClientResponse()
		{
			Client = clientDto
		});
	}
}
