using Microsoft.EntityFrameworkCore;

namespace Data
{
	public class ReadDbContext(DbContextOptions<ReadDbContext> options) : BaseDbContext(options)
	{
        
	}
}
