namespace Core
{
	public interface IRepository<TEntity> : IReadRepository<TEntity>
		where TEntity : class
	{
		Task AddAsync(TEntity entity);
		Task UpdateAsync(TEntity entity);
		Task DeleteAsync(TEntity entity);
	}
}
