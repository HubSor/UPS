using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace UPS
{
    public class UPSContext : DbContext
    {
        public UPSContext(DbContextOptions<UPSContext> options) : base(options) { }
        public DbSet<Product>? Products { get; set; }
    }
}
