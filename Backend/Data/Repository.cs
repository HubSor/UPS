namespace Data
{
	public class Repository<TEntity> : IRepository<TEntity>
		where TEntity : class
	{
		protected readonly UnitOfWork context;
		public Repository(UnitOfWork context)
		{
			this.context = context;
		}

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
