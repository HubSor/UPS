namespace Core
{
    public interface IRepository<TEntity>
    {
        TEntity Add(TEntity entity);
        IEnumerable<TEntity> GetAll();
    }
}
