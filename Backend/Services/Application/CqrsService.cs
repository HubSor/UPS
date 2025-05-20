using MassTransit;
using Messages.Commands;
using Messages.Queries;
using Messages.Responses;
using Services.Infrastructure;

namespace Services.Application;
public class CqrsService : ICqrsService
{
    private readonly IQueryBus _queryBus;
    private readonly ICommandBus _commandBus;

    public CqrsService(IQueryBus queryBus, ICommandBus commandBus)
    {
        _queryBus = queryBus;
        _commandBus = commandBus;
    }

    public async Task<ApiResponse<TResponse>> PerformQueryAsync<TQuery, TResponse>(TQuery query)
        where TQuery : Query
        where TResponse : class
    {
        var client = _queryBus.CreateRequestClient<TQuery>();
        var response = await client.GetResponse<ApiResponse<TResponse>>(query);
        return response.Message;
    }

    public async Task<ApiResponse<TResponse>> PerformCommandAsync<TCommand, TResponse>(TCommand command)
        where TCommand : Command
        where TResponse : class
    {
        var client = _commandBus.CreateRequestClient<TCommand>();
        var response = await client.GetResponse<ApiResponse<TResponse>>(command);
        return response.Message;
    }
}