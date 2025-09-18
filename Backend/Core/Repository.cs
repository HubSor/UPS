using Data;

namespace Core
{
	public class Repository<TEntity>(BaseUnitOfWork context) : IRepository<TEntity>
		where TEntity : class
	{
		protected readonly BaseUnitOfWork context = context;

        public async Task AddAsync(TEntity entity)
		{
			await context.Set<TEntity>().AddAsync(entity);
			await context.SaveChangesAsync();
		}

		public async Task DeleteAsync(TEntity entity)
		{
			context.Set<TEntity>().Remove(entity);
			await context.SaveChangesAsync();
		}

		public async Task UpdateAsync(TEntity entity)
		{
			context.Set<TEntity>().Update(entity);
			await context.SaveChangesAsync();
		}

		public IQueryable<TEntity> GetAll()
		{
			return context.Set<TEntity>().AsQueryable();
		}
	}
}
