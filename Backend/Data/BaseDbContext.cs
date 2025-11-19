using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Data
{
	public abstract class BaseDbContext : DbContext
	{
		public bool InTransaction => Database.CurrentTransaction != null;
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<ProductStatus> ProductStatuses { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<SubProduct> SubProducts { get; set; }
		public DbSet<SubProductInProduct> SubProductsInProducts { get; set; }
		public DbSet<Parameter> Parameters { get; set; }
		public DbSet<ParameterOption> ParameterOptions { get; set; }
		public DbSet<ParameterType> ParameterTypes { get; set; }
		public DbSet<Sale> Sales { get; set; }
		public DbSet<SaleParameter> SaleParameters { get; set; }
		public DbSet<SubProductInSale> SubProductsInSales { get; set; }
		public DbSet<AddressType> AddressTypes { get; set; }
		public DbSet<ClientAddress> ClientAddresses { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<PersonClient> PersonClients { get; set; }
		public DbSet<CompanyClient> CompanyClients { get; set; }
		
		public BaseDbContext(DbContextOptions options) : base(options)
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		}
		
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
