using Microsoft.EntityFrameworkCore;

namespace Core
{
	public class UnitOfWork : DbContext, IUnitOfWork
	{
		public bool InTransaction => Database.CurrentTransaction != null;
		
		public UnitOfWork() : base()
		{
		}
		
		public async Task BeginTransasctionAsync(CancellationToken cancellationToken = default)
		{
			await Database.BeginTransactionAsync(cancellationToken);
		}
		
		public async Task CommitTransasctionAsync(CancellationToken cancellationToken = default)
		{
			await Database.CommitTransactionAsync(cancellationToken);
		}
		
		public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
		{
			await Database.RollbackTransactionAsync(cancellationToken);
		}
		
		public async Task<int> FlushAsync(CancellationToken cancellationToken = default)
		{
			return await base.SaveChangesAsync(cancellationToken);
		}
	}
}
