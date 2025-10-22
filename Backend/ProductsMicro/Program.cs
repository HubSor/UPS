using Core.Web;
using ProductsMicro.Data;

var builder = WebApplication.CreateBuilder(args);

Installer.InstallCommonMicroServices<ProductsUnitOfWork>(builder);

var app = builder.Build();

try 
{
	using var scope = app.Services.CreateScope();
	{
		var context = scope.ServiceProvider.GetRequiredService<ProductsUnitOfWork>();
		ProductsDataInitializer.Initialize(context);
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