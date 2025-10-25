using Core.Web;
using SalesMicro.Data;

var builder = WebApplication.CreateBuilder(args);

Installer.InstallCommonMicroServices<SalesUnitOfWork>(builder);

var app = builder.Build();

try 
{
	using var scope = app.Services.CreateScope();
	{
		var context = scope.ServiceProvider.GetRequiredService<SalesUnitOfWork>();
		SalesDataInitializer.Initialize(context);
	}
}
catch (Exception)
{
	Console.WriteLine("Database initialization error");
	throw;
}

app.Run();

public partial class Program {}