using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Core
{
    public class UPSContext : DbContext
    {
        public UPSContext(DbContextOptions<UPSContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
