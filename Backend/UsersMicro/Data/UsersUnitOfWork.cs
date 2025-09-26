using Data;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace UsersMicro.Data
{
	public class UsersUnitOfWork(DbContextOptions options) : BaseUnitOfWork(options), IUnitOfWork
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<ClientAddress> ClientAddresses { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<PersonClient> PersonClients { get; set; }
		public DbSet<CompanyClient> CompanyClients { get; set; }
	}
}
