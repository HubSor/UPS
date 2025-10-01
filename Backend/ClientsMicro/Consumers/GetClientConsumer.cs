using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ClientsMicro.Consumers;

public abstract class GetClientConsumer<T>(ILogger<GetClientConsumer<T>> _logger, IRepository<T> clients, IUnitOfWork unitOfWork)
	: TransactionConsumer<GetClientOrder, GetClientResponse>(unitOfWork, _logger)
	where T : Client
{
	protected readonly IRepository<T> clients = clients;
	protected T client = default!;
	protected ClientDto clientDto = default!;

	public abstract Task<T?> GetClientAsync(ConsumeContext<GetClientOrder> context);
	public abstract ClientDto CreateClientDto(ConsumeContext<GetClientOrder> context);

	public override async Task<bool> PreTransaction(ConsumeContext<GetClientOrder> context)
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

	public override Task InTransaction(ConsumeContext<GetClientOrder> context)
	{
		logger.LogInformation("Creating found client Dto");
		clientDto = CreateClientDto(context);
		return Task.CompletedTask;
	}
}

public class GetPersonClientConsumer(
    ILogger<GetPersonClientConsumer> logger,
	IRepository<PersonClient> clients,
	IUnitOfWork unitOfWork
) : GetClientConsumer<PersonClient>(logger, clients, unitOfWork)
{
    public override async Task<PersonClient?> GetClientAsync(ConsumeContext<GetClientOrder> context)
	{
		return await clients.GetAll()
			.FirstOrDefaultAsync(x => x.Id == context.Message.ClientId);
	}

	public override ClientDto CreateClientDto(ConsumeContext<GetClientOrder> context)
	{
		return client.ToDto();
	}

	public override async Task PostTransaction(ConsumeContext<GetClientOrder> context)
	{
		await RespondAsync(context, new GetClientResponse()
		{
			PersonClient = clientDto as PersonClientDto ?? throw new UPSException("Illegal client type")
		});
	}
}

public class GetCompanyClientConsumer(ILogger<GetCompanyClientConsumer> logger, IRepository<CompanyClient> clients, IUnitOfWork unitOfWork)
	: GetClientConsumer<CompanyClient>(logger, clients, unitOfWork)
{
    public override async Task<CompanyClient?> GetClientAsync(ConsumeContext<GetClientOrder> context)
	{
		return await clients.GetAll()
			.FirstOrDefaultAsync(x => x.Id == context.Message.ClientId);
	}

	public override ClientDto CreateClientDto(ConsumeContext<GetClientOrder> context)
	{
		return client.ToDto();
	}

	public override async Task PostTransaction(ConsumeContext<GetClientOrder> context)
	{
		await RespondAsync(context, new GetClientResponse()
		{
			CompanyClient = clientDto as CompanyClientDto ?? throw new UPSException("Illegal client type")
		});
	}
}