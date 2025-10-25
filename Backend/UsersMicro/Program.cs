using UsersMicro.Data;
using UsersMicro.Services;
using Core.Web;

var builder = WebApplication.CreateBuilder(args);

Installer.InstallCommonMicroServices<UsersUnitOfWork>(builder);
builder.Services.AddScoped<IPasswordService, PasswordService>();

var app = builder.Build();

try 
{
	using var scope = app.Services.CreateScope();
	{
		var context = scope.ServiceProvider.GetRequiredService<UsersUnitOfWork>();
		var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
		UsersDataInitializer.Initialize(context, passwordService);
	}
}
catch (Exception)
{
	Console.WriteLine("Database initialization error");
	throw;
}

app.Run();

public partial class Program {}