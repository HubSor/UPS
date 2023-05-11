using Microsoft.EntityFrameworkCore;

namespace Core
{
    public class UPSContext : DbContext
    {
        public UPSContext(DbContextOptions<UPSContext> options) : base(options) { }
    }
}
