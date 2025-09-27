namespace Core.Data
{
	public interface IRepository<TEntity>
		where TEntity : class
	{
		IQueryable<TEntity> GetAll();
		Task AddAsync(TEntity entity);
		Task UpdateAsync(TEntity entity);
		Task DeleteAsync(TEntity entity);
	}
}
