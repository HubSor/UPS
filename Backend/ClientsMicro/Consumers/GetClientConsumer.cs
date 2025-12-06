using Core;
using Core.Data;
using Core.Dtos;
using Core.Messages;
using Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ClientsMicro.Consumers;

public class GetClientConsumer(ILogger<GetClientConsumer> _logger, IRepository<Client> clients, IUnitOfWork unitOfWork)
	: TransactionConsumer<GetClientOrder, GetClientResponse>(unitOfWork, _logger)
{
	protected Client client = default!;
	protected ClientDto clientDto = default!;

	public override async Task<bool> PreTransaction(ConsumeContext<GetClientOrder> context)
	{
		var foundClient = await clients.GetAll().FirstOrDefaultAsync(x => x.Id == context.Message.ClientId);
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
		clientDto = client is PersonClient personClient ? personClient.ToDto() : client is CompanyClient companyClient ? companyClient.ToDto() : throw new Exception("Invalid client type");
		return Task.CompletedTask;
	}

	public override async Task PostTransaction(ConsumeContext<GetClientOrder> context)
	{
		await RespondAsync(context, new GetClientResponse()
		{
			CompanyClient = clientDto is CompanyClientDto companyClientDto ? companyClientDto : null,
			PersonClient = clientDto is PersonClientDto personClientDto ? personClientDto : null,
		});
	}
}