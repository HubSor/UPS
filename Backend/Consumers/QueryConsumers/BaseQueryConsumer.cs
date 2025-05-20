using MassTransit;
using Core;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Consumers.QueryConsumers;
public abstract class BaseQueryConsumer<Order, Response> : IConsumer<Order>
	where Order : class 
	where Response : class
{
	protected readonly ILogger<BaseQueryConsumer<Order, Response>> logger;
	
	public BaseQueryConsumer(ILogger<BaseQueryConsumer<Order, Response>> logger)
	{
		this.logger = logger;
	}
		
	public abstract Task Consume(ConsumeContext<Order> context);

	public virtual async Task RespondWithValidationFailAsync(ConsumeContext<Order> context, string propertyName, string ErrorMessage)
	{
		await context.RespondAsync(new ApiResponse<Response>(new ValidationFailure(propertyName, ErrorMessage)));
		return;
	}
	
	public virtual async Task RespondAsync(ConsumeContext<Order> context, Response response)
	{
		await context.RespondAsync(new ApiResponse<Response>(response));
	}
}