using Core;
using Dtos.Clients;
using MassTransit;
using Messages.Queries;
using Messages.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Query;
public abstract class FindClientConsumer<T, O, R> : BaseQueryConsumer<O, R>
	where T : Client
	where O : FindClientQuery
	where R : FindClientResponse
{
	protected readonly IReadRepository<T> clients;
	
	public FindClientConsumer(ILogger<FindClientConsumer<T, O, R>> logger, IReadRepository<T> clients)
		: base(logger)
	{
		this.clients = clients;
	}
	
	public abstract Task<T?> GetClientAsync(ConsumeContext<O> context);
	public abstract ClientDto CreateClientDto(T client);
	public abstract Task Respond(ConsumeContext<O> context, ClientDto clientDto);

	public override async Task Consume(ConsumeContext<O> context)
	{
		var foundClient = await GetClientAsync(context);

		if (foundClient == null)
		{
			await RespondWithValidationFailAsync(context, "Identifier", "Nie znaleziono klienta");
			return;
		}

		logger.LogInformation("Creating found client Dto");
		var clientDto = CreateClientDto(foundClient);

		await Respond(context, clientDto);
	}
}

public class FindPersonClientConsumer : FindClientConsumer<PersonClient, FindPersonClientQuery, FindPersonClientResponse>
{
	public FindPersonClientConsumer(ILogger<FindPersonClientConsumer> logger, IRepository<PersonClient> clients)
		: base(logger, clients)
	{
	}

	public override async Task<PersonClient?> GetClientAsync(ConsumeContext<FindPersonClientQuery> context)
	{
		return await clients.GetAll()
			.FirstOrDefaultAsync(x => x.Pesel == context.Message.Identifier.ToUpper().Trim() && !x.Deleted);
	}

	public override ClientDto CreateClientDto(PersonClient client)
	{
		return new PersonClientDto(client);
	}

	public override async Task Respond(ConsumeContext<FindPersonClientQuery> context, ClientDto clientDto)
	{
		await RespondAsync(context, new FindPersonClientResponse()
		{
			PersonClient = clientDto as PersonClientDto ?? throw new UPSException("Illegal client type")
		});
	}
}

public class FindCompanyClientConsumer : FindClientConsumer<CompanyClient, FindCompanyClientQuery, FindCompanyClientResponse>
{
	public FindCompanyClientConsumer(ILogger<FindCompanyClientConsumer> logger, IRepository<CompanyClient> clients)
		: base(logger, clients)
	{
	}

	public override async Task<CompanyClient?> GetClientAsync(ConsumeContext<FindCompanyClientQuery> context)
	{
		return await clients.GetAll()
			.FirstOrDefaultAsync(x => !x.Deleted && (x.Nip == context.Message.Identifier.ToUpper().Trim() ||
				x.Regon == context.Message.Identifier.ToUpper().Trim()));
	}

	public override ClientDto CreateClientDto(CompanyClient client)
	{
		return new CompanyClientDto(client);
	}

	public override async Task Respond(ConsumeContext<FindCompanyClientQuery> context, ClientDto clientDto)
	{
		await RespondAsync(context, new FindCompanyClientResponse()
		{
			CompanyClient = clientDto as CompanyClientDto ?? throw new UPSException("Illegal client type")
		});
	}
}