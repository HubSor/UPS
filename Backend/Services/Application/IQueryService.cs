using Messages.Queries;
using Messages.Responses;

namespace Services.Application;
public interface IQueryService
{
    public Task<ApiResponse<TResponse>> PerformQueryAsync<TQuery, TResponse>(TQuery query)
        where TQuery : Query
        where TResponse : class;
}
