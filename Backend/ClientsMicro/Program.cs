using Core;
using FluentValidation;
using MassTransit;
using WebCommons;
using Microsoft.EntityFrameworkCore;
using Core.Web;
using ClientsMicro.Data;
using Core.Data;

var builder = WebApplication.CreateBuilder(args);

try
{
	builder.Services.AddDbContext<ClientsUnitOfWork>(options => 
	{
		options.UseNpgsql(
			builder.Configuration.GetConnectionString("Users_Connection"),
			op => {
				op.MigrationsAssembly(typeof(BaseUnitOfWork).Assembly.FullName);
				op.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
			}
		);
	});
}
catch (Exception)
{
	Console.WriteLine("Database connection error");
	throw;
}

builder.Services.AddControllersWithViews(x => x.Filters.Add<ExceptionFilter>());

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, ClientsUnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

builder.Services.AddMediator(mrc =>
{
	mrc.ConfigureMediator((context, cfg) =>
	{
		cfg.UseSendFilter(typeof(ValidationFilter<>), context);
	});

	mrc.AddConsumers(typeof(BaseConsumer<,>).Assembly);
});

Installer.InstallAuth(builder.Services);

Installer.InstallDataProtection(builder.Services);

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


app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();

Installer.EnableAuth(app);

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=test}/{action=get}"
);

app.Run();

public partial class Program {}