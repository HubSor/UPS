using Messages.Commands;
using Messages.Queries;
using Messages.Responses;

namespace Services.Application;
public interface ICqrsService
{
    public Task<ApiResponse<TResponse>> PerformQueryAsync<TQuery, TResponse>(TQuery query)
        where TQuery : Query
        where TResponse : class;

    public Task<ApiResponse<TResponse>> PerformCommandAsync<TCommand, TResponse>(TCommand command)
        where TCommand : Command
        where TResponse : class;
}
