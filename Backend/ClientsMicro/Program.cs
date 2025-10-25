using ClientsMicro.Data;
using Core.Web;

var builder = WebApplication.CreateBuilder(args);

Installer.InstallCommonMicroServices<ClientsUnitOfWork>(builder);

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

app.Run();

public partial class Program {}