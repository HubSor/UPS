using Data;

namespace Core
{
	public class ReadRepository<TEntity>(ReadDbContext readDbContext) : IReadRepository<TEntity>
		where TEntity : class
	{
		protected readonly ReadDbContext context = readDbContext;

        public IQueryable<TEntity> GetAll()
		{
			return context.Set<TEntity>().AsQueryable();
		}
	}
}
