using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace SalesMicro.Data
{
	public class SalesUnitOfWork(DbContextOptions options) : BaseUnitOfWork(options), IUnitOfWork
	{
		public DbSet<Sale> Sales { get; set; }
		public DbSet<SaleParameter> SaleParameters { get; set; }
		public DbSet<SubProductInSale> SubProductsInSales { get; set; }
	}
}
