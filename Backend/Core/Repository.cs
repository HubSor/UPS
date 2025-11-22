using Data;

namespace Core
{
	public class Repository<TEntity>(UnitOfWork uow, ReadDbContext readDbContext) : ReadRepository<TEntity>(readDbContext), IRepository<TEntity>
		where TEntity : class
	{
		protected readonly UnitOfWork unitOfWork = uow;

        public async Task AddAsync(TEntity entity)
		{
			await unitOfWork.Set<TEntity>().AddAsync(entity);
			await unitOfWork.SaveChangesAsync();
		}

		public async Task DeleteAsync(TEntity entity)
		{
			unitOfWork.Set<TEntity>().Remove(entity);
			await unitOfWork.SaveChangesAsync();
		}

		public async Task UpdateAsync(TEntity entity)
		{
			unitOfWork.Set<TEntity>().Update(entity);
			await unitOfWork.SaveChangesAsync();
		}
	}
}
