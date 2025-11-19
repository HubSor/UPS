namespace Core
{
	public interface IReadRepository<TEntity>
		where TEntity : class
	{
		IQueryable<TEntity> GetAll();
	}
}
