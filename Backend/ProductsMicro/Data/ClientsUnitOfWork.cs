using ClientsMicro.Models;
using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace ClientsMicro.Data
{
	public class ClientsUnitOfWork(DbContextOptions options) : BaseUnitOfWork(options), IUnitOfWork
	{
		public DbSet<ClientAddress> ClientAddresses { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<PersonClient> PersonClients { get; set; }
		public DbSet<CompanyClient> CompanyClients { get; set; }
	}
}
