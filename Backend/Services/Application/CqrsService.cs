using MassTransit.Mediator;
using Messages.Commands;
using Messages.Queries;
using Messages.Responses;

namespace Services.Application;
public class CqrsService(IMediator mediator) : ICqrsService
{
    public async Task<ApiResponse<TResponse>> PerformQueryAsync<TQuery, TResponse>(TQuery query)
        where TQuery : Query
        where TResponse : class
    {
        var client = mediator.CreateRequestClient<TQuery>();
        var response = await client.GetResponse<ApiResponse<TResponse>>(query);
        return response.Message;
    }

    public async Task<ApiResponse<TResponse>> PerformCommandAsync<TCommand, TResponse>(TCommand command)
        where TCommand : Command
        where TResponse : class
    {
        var client = mediator.CreateRequestClient<TCommand>();
        var response = await client.GetResponse<ApiResponse<TResponse>>(command);
        return response.Message;
    }
}