using Core;
using Dtos;
using Dtos.Clients;
using MassTransit;
using Messages.Clients;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace Consumers.Clients;
public abstract class ListClientsConsumer<T, O, D, R> : BaseConsumer<O, R>
	where T : Client
	where O : ListClientsOrder
	where D : ClientDto
	where R : class
{
	protected readonly IRepository<T> clients;
	
	public ListClientsConsumer(ILogger<ListClientsConsumer<T, O, D, R>> logger, IRepository<T> clients)
		: base(logger)
	{
		this.clients = clients;
	}

	protected abstract D GetClientDto(T client);
	protected abstract Task RespondAsync(ConsumeContext<O> context, ICollection<D> dtos, int count);

	public override async Task Consume(ConsumeContext<O> context)
	{
		var count = clients.GetAll().Count();
		var list = clients.GetAll()
			.OrderByDescending(x => x.Id)
			.Skip(context.Message.Pagination.PageIndex * context.Message.Pagination.PageSize)
			.Take(context.Message.Pagination.PageSize)
			.Select(GetClientDto)
			.ToList();
			
		await RespondAsync(context, list, count);
	}
}

public class ListPersonClientsConsumer : ListClientsConsumer<PersonClient, ListPersonClientsOrder, PersonClientDto, ListPersonClientsResponse>
{
	public ListPersonClientsConsumer(ILogger<ListPersonClientsConsumer> logger, IRepository<PersonClient> clients)
	: base(logger, clients)
	{
	}

	protected override PersonClientDto GetClientDto(PersonClient client) => new (client);

	protected override async Task RespondAsync(ConsumeContext<ListPersonClientsOrder> context, ICollection<PersonClientDto> dtos, int count)
	{
		logger.LogInformation("Listing clients of type {ClientType}", nameof(PersonClient));
		await RespondAsync(context, new ListPersonClientsResponse()
		{
			Clients = new PagedList<PersonClientDto>(dtos, count,
				context.Message.Pagination.PageIndex, context.Message.Pagination.PageSize)
		});
	}
}

public class ListCompanyClientsConsumer : ListClientsConsumer<CompanyClient, ListCompanyClientsOrder, CompanyClientDto, ListCompanyClientsResponse>
{
	public ListCompanyClientsConsumer(ILogger<ListCompanyClientsConsumer> logger, IRepository<CompanyClient> clients)
	: base(logger, clients)
	{
	}

	protected override CompanyClientDto GetClientDto(CompanyClient client) => new(client);

	protected override async Task RespondAsync(ConsumeContext<ListCompanyClientsOrder> context, ICollection<CompanyClientDto> dtos, int count)
	{
		logger.LogInformation("Listing clients of type {ClientType}", nameof(CompanyClient));
		await RespondAsync(context, new ListCompanyClientsResponse()
		{
			Clients = new PagedList<CompanyClientDto>(dtos, count,
				context.Message.Pagination.PageIndex, context.Message.Pagination.PageSize)
		});
	}
}