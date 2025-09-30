using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
	public abstract class BaseUnitOfWork : DbContext, IUnitOfWork
	{
		public bool InTransaction => Database.CurrentTransaction != null;

		public DbSet<AddressType> AddressTypes { get; set; }

		public DbContext Context => this;
		
		public BaseUnitOfWork(DbContextOptions options) : base(options)
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		}
		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.IsFullyTrusted);
			foreach (var assembly in assemblies)
			{
				modelBuilder.ApplyConfigurationsFromAssembly(assembly);
			}
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
