using Core;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Consumers;

public abstract class TransactionConsumer<Order, Response> : BaseConsumer<Order, Response>
	where Order : class
	where Response : class
{
	protected readonly IUnitOfWork unitOfWork;
	protected readonly ILogger<TransactionConsumer<Order, Response>> logger;
	public TransactionConsumer(IUnitOfWork unitOfWork, ILogger<TransactionConsumer<Order,Response>> logger)
	{
		this.unitOfWork = unitOfWork;
		this.logger = logger;
	}
	public override async Task Consume(ConsumeContext<Order> context)
	{
		if (await PreTransaction(context))
		{
			try
			{
				// todo logowanie
				await unitOfWork.BeginTransasctionAsync();
				await Transaction(context);
				await unitOfWork.FlushAsync();
				await unitOfWork.CommitTransasctionAsync();
			}
			catch (Exception ex)
			{
				await unitOfWork.RollbackTransactionAsync();
				logger.LogInformation(ex, "Exception in TransactionConsumer");
			}
			
			await PostTransaction(context);
		}
		else
		{
			await InsteadOfTransaction(context);
		}
	}

	public virtual Task<bool> PreTransaction(ConsumeContext<Order> context) => Task.FromResult(true);
	public virtual Task Transaction(ConsumeContext<Order> context) => Task.CompletedTask;
	public virtual Task PostTransaction(ConsumeContext<Order> context) => Task.CompletedTask;
	public virtual Task InsteadOfTransaction(ConsumeContext<Order> context) => Task.CompletedTask;
}