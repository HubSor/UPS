using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicro.Data
{
	public class ProductsUnitOfWork(DbContextOptions options) : BaseUnitOfWork(options), IUnitOfWork
	{
		public DbSet<ProductStatus> ProductStatuses { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<SubProduct> SubProducts { get; set; }
		public DbSet<SubProductInProduct> SubProductsInProducts { get; set; }
		public DbSet<Parameter> Parameters { get; set; }
		public DbSet<ParameterOption> ParameterOptions { get; set; }
		public DbSet<ParameterType> ParameterTypes { get; set; }
	}
}
