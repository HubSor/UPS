using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class UPSWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
	where TProgram : class
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UnitOfWork>));
			if (dbContextDescriptor is not null)
				services.Remove(dbContextDescriptor);

			var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
			if (dbConnectionDescriptor is not null)
				services.Remove(dbConnectionDescriptor);
				
			var conn = new SqliteConnection($"DataSource={Guid.NewGuid()}-integration-tests.db");
			conn.Open();

			services.AddDbContext<UnitOfWork>((options) =>
			{
				options.UseSqlite(conn);
			});

			var serviceProvider = services.BuildServiceProvider();

			using var scope = serviceProvider.CreateScope();

			var uow = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
			var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

			try 
			{
				DataInitializer.InitializeTest(uow, passwordService);
			}
			catch (Exception)
			{
				Console.WriteLine("Database initialization error");
				throw;
			}
		});

		builder.UseEnvironment("Development");
	}
}