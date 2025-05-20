using Data;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Consumers;

public abstract class BaseCommandConsumer<Order, Response> : BaseQueryConsumer<Order, Response>
	where Order : class
	where Response : class
{
	protected readonly IUnitOfWork unitOfWork;
	public BaseCommandConsumer(IUnitOfWork unitOfWork, ILogger<BaseQueryConsumer<Order,Response>> logger)
		: base(logger)
	{
		this.unitOfWork = unitOfWork;
	}
	
	public override async Task Consume(ConsumeContext<Order> context)
	{
		logger.LogTrace("Starting pre transaction action of {ConsumerName}", GetType().Name);
		if (await PreTransaction(context))
		{
			try
			{
				logger.LogTrace("Beggining transaction of {ConsumerName}", GetType().Name);
				await unitOfWork.BeginTransasctionAsync();
				await InTransaction(context);
				await unitOfWork.FlushAsync();
				await unitOfWork.CommitTransasctionAsync();
				logger.LogTrace("Committed transaction of {ConsumerName}", GetType().Name);

				logger.LogTrace("Starting post transaction action of {ConsumerName}", GetType().Name);
				await PostTransaction(context);
			}
			catch (Exception ex)
			{
				await unitOfWork.RollbackTransactionAsync();
				logger.LogInformation(ex, "Exception in {ConsumerName}", GetType().Name);
				throw;
			}
		}
		else
		{
			await InsteadOfTransaction(context);
		}
	}

	public virtual Task<bool> PreTransaction(ConsumeContext<Order> context) => Task.FromResult(true);
	public virtual Task InTransaction(ConsumeContext<Order> context) => Task.CompletedTask;
	public virtual Task PostTransaction(ConsumeContext<Order> context) => Task.CompletedTask;
	public virtual Task InsteadOfTransaction(ConsumeContext<Order> context) => Task.CompletedTask;
}