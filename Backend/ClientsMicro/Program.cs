using ClientsMicro.Data;
using Core.Web;

var builder = WebApplication.CreateBuilder(args);

Installer.InstallCommonServices<ClientsUnitOfWork>(builder);

var app = builder.Build();

try 
{
	using var scope = app.Services.CreateScope();
	{
		var context = scope.ServiceProvider.GetRequiredService<ClientsUnitOfWork>();
		ClientsDataInitializer.Initialize(context);
	}
}
catch (Exception)
{
	Console.WriteLine("Database initialization error");
	throw;
}

Installer.EnableCommonServices(app);

app.Run();

public partial class Program {}