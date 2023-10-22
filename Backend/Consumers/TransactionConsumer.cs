using Data;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Consumers;

public abstract class TransactionConsumer<Order, Response> : BaseConsumer<Order, Response>
	where Order : class
	where Response : class
{
	protected readonly IUnitOfWork unitOfWork;
	public TransactionConsumer(IUnitOfWork unitOfWork, ILogger<BaseConsumer<Order,Response>> logger)
		: base(logger)
	{
		this.unitOfWork = unitOfWork;
	}
	
	public override async Task Consume(ConsumeContext<Order> context)
	{
		if (await PreTransaction(context))
		{
			try
			{
				// todo logowanie
				await unitOfWork.BeginTransasctionAsync();
				await InTransaction(context);
				await unitOfWork.FlushAsync();
				await unitOfWork.CommitTransasctionAsync();
				
				await PostTransaction(context);
			}
			catch (Exception ex)
			{
				await unitOfWork.RollbackTransactionAsync();
				logger.LogInformation(ex, "Exception in TransactionConsumer");
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