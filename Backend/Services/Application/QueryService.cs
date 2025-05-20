using MassTransit;
using Messages.Queries;
using Messages.Responses;
using Services.Infrastructure;

namespace Services.Application;
public class QueryService : IQueryService
{
    private readonly IQueryBus _queryBus;

    public QueryService(IQueryBus queryBus)
    {
        _queryBus = queryBus;
    }

    public async Task<ApiResponse<TResponse>> PerformQueryAsync<TQuery, TResponse>(TQuery query)
        where TQuery : Query
        where TResponse : class
    {
        var client = _queryBus.CreateRequestClient<TQuery>();
        var response = await client.GetResponse<ApiResponse<TResponse>>(query);
        return response.Message;
    }
}