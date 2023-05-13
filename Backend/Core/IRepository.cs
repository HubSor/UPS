namespace Core
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
