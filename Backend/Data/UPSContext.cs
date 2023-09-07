using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Core
{
	public class UPSContext : DbContext
	{
		public UPSContext(DbContextOptions options) : base(options) {}
		public DbSet<User> Users { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.IsFullyTrusted);
			foreach (var assembly in assemblies)
			{
				modelBuilder.ApplyConfigurationsFromAssembly(assembly);
			}
		}
	}
}
