using Data;
using Microsoft.EntityFrameworkCore;

namespace Core
{
	public class Repository<TEntity>(IUnitOfWork context) : IRepository<TEntity>
		where TEntity : class
	{
		protected readonly DbContext context = context.Context;

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
