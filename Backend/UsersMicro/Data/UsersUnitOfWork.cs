using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using UsersMicro.Models;

namespace UsersMicro.Data
{
	public class UsersUnitOfWork(DbContextOptions options) : BaseUnitOfWork(options), IUnitOfWork
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
	}
}
