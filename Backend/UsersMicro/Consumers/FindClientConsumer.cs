using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UsersMicro.Models;
using WebCommons;

namespace UsersMicro.Consumers;

public abstract class FindClientConsumer<T, O, R> : TransactionConsumer<O, R>
	where T : Client
	where O : FindClientOrder
	where R : FindClientResponse
{
	protected readonly IRepository<T> clients;
	protected T client = default!;
	protected ClientDto clientDto = default!;
	
	public FindClientConsumer(ILogger<FindClientConsumer<T, O, R>> logger, IRepository<T> clients, IUnitOfWork unitOfWork)
		: base(unitOfWork, logger)
	{
		this.clients = clients;
	}
	
	public abstract Task<T?> GetClientAsync(ConsumeContext<O> context);
	public abstract ClientDto CreateClientDto(ConsumeContext<O> context);

	public override async Task<bool> PreTransaction(ConsumeContext<O> context)
	{
		var foundClient = await GetClientAsync(context);

		if (foundClient == null)
		{
			await RespondWithValidationFailAsync(context, "Identifier", "Nie znaleziono klienta");
			return false;
		}

		client = foundClient;

		return true;
	}

	public override Task InTransaction(ConsumeContext<O> context)
	{
		logger.LogInformation("Creating found client Dto");
		clientDto =  CreateClientDto(context);
		return Task.CompletedTask;
	}
}

public class FindPersonClientConsumer : FindClientConsumer<PersonClient, FindPersonClientOrder, FindPersonClientResponse>
{
	public FindPersonClientConsumer(ILogger<FindPersonClientConsumer> logger, IRepository<PersonClient> clients, IUnitOfWork unitOfWork)
		: base (logger, clients, unitOfWork)
	{
		
	}

	public override async Task<PersonClient?> GetClientAsync(ConsumeContext<FindPersonClientOrder> context)
	{
		return await clients.GetAll()
			.FirstOrDefaultAsync(x => x.Pesel == context.Message.Identifier.ToUpper().Trim() && !x.Deleted);
	}

	public override ClientDto CreateClientDto(ConsumeContext<FindPersonClientOrder> context)
	{
		return client.ToDto();
	}

	public override async Task PostTransaction(ConsumeContext<FindPersonClientOrder> context)
	{
		await RespondAsync(context, new FindPersonClientResponse()
		{
			PersonClient = clientDto as PersonClientDto ?? throw new UPSException("Illegal client type")
		});
	}
}

public class FindCompanyClientConsumer : FindClientConsumer<CompanyClient, FindCompanyClientOrder, FindCompanyClientResponse>
{
	public FindCompanyClientConsumer(ILogger<FindCompanyClientConsumer> logger, IRepository<CompanyClient> clients, IUnitOfWork unitOfWork)
		: base(logger, clients, unitOfWork)
	{

	}

	public override async Task<CompanyClient?> GetClientAsync(ConsumeContext<FindCompanyClientOrder> context)
	{
		return await clients.GetAll()
			.FirstOrDefaultAsync(x => !x.Deleted && (x.Nip == context.Message.Identifier.ToUpper().Trim() ||
				x.Regon == context.Message.Identifier.ToUpper().Trim()));
	}

	public override ClientDto CreateClientDto(ConsumeContext<FindCompanyClientOrder> context)
	{
		return client.ToDto();
	}

	public override async Task PostTransaction(ConsumeContext<FindCompanyClientOrder> context)
	{
		await RespondAsync(context, new FindCompanyClientResponse()
		{
			CompanyClient = clientDto as CompanyClientDto ?? throw new UPSException("Illegal client type")
		});
	}
}