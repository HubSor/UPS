namespace Data
{
	public interface IUnitOfWork : IDisposable
	{
		public Task BeginTransasctionAsync(CancellationToken cancellationToken = default);
		public Task CommitTransasctionAsync(CancellationToken cancellationToken = default);
		public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
		public Task<int> FlushAsync(CancellationToken cancellationToken = default);
	}
}
