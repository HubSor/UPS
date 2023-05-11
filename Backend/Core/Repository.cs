using Microsoft.EntityFrameworkCore;

namespace Core
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected UPSContext context;
        protected DbSet<TEntity> entities;
        public Repository(UPSContext context)
        {
            this.context = context;
            entities = context.Set<TEntity>();
        }

        public virtual TEntity Add(TEntity entity)
        {
            return entities.Add(entity).Entity;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return entities;
        }
    }
}
