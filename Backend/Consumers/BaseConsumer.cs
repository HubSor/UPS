using MassTransit;
using Core;
using FluentValidation.Results;

namespace Consumers;

public abstract class BaseConsumer<Order, Response> : IConsumer<Order>
    where Order : class 
    where Response : class
{
    public abstract Task Consume(ConsumeContext<Order> context);

    public virtual async Task ValidationFailAsync(ConsumeContext<Order> context, string propertyName, string ErrorMessage)
    {
        await context.RespondAsync(new ApiResponse<Response>(new ValidationFailure(propertyName, ErrorMessage)));
        return;
    }
}