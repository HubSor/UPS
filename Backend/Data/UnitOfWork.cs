using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Data
{
	public class UnitOfWork(DbContextOptions options) : BaseDbContext(options), IUnitOfWork
	{
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
