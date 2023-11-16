using Core;
using Data;
using MassTransit;
using Messages.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Clients;
public class UpsertClientConsumer : TransactionConsumer<UpsertClientOrder, UpsertClientResponse>
{
	private readonly IRepository<Client> clients;
	private Client? client;
	private bool clientFound;
	
	public UpsertClientConsumer(ILogger<UpsertClientConsumer> logger, IRepository<Client> clients, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.clients = clients;
	}

	public override async Task<bool> PreTransaction(ConsumeContext<UpsertClientOrder> context)
	{
		if (context.Message.ClientId.HasValue)
		{
			client = await clients.GetAll().FirstOrDefaultAsync(x => x.Id == context.Message.ClientId && !x.Deleted);
			if (client != null && ((client is PersonClient && context.Message.IsCompany) || (client is CompanyClient && !context.Message.IsCompany)))
			{
				await RespondWithValidationFailAsync(context, "ClientId", "Niedozwolona próba zmiany typu klienta");
				return false;
			}
			if (client != null)
				clientFound = true;
		}

		client ??= context.Message.IsCompany ? new CompanyClient() : new PersonClient();
		
		return true;
	}

	public override async Task InTransaction(ConsumeContext<UpsertClientOrder> context)
	{
		if (client == null)
			throw new UPSException("No client");
			
		client.PhoneNumber = context.Message.PhoneNumber;
		client.Email = context.Message.Email;
		if (client is PersonClient personClient)
		{
			personClient.Pesel = context.Message.Pesel;
			personClient.FirstName = context.Message.FirstName!;
			personClient.LastName = context.Message.LastName!;
		}
		if (client is CompanyClient companyClient)
		{
			companyClient.CompanyName = context.Message.CompanyName!;
			companyClient.Nip = context.Message.Nip;
			companyClient.Regon = context.Message.Regon;
		}
			
		if (clientFound)
			await clients.UpdateAsync(client);
		else
			await clients.AddAsync(client); 
	}

    public override async Task PostTransaction(ConsumeContext<UpsertClientOrder> context)
    {
        await RespondAsync(context, new UpsertClientResponse());
    }
}