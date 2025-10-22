using System.Reflection;
using Core.Web;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(x => x.Filters.Add<ExceptionFilter>());
builder.Services.AddCors(op =>
{
	op.AddPolicy("AllowFrontend", pol =>
	{
		pol.WithOrigins(builder.Configuration.GetValue<string>("FRONTEND_ORIGIN") ?? "", "https://localhost:3000", "http://localhost:3000")
			.AllowAnyMethod().AllowAnyHeader().AllowCredentials();
		pol.SetPreflightMaxAge(TimeSpan.FromMinutes(15));
	});
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
	var requestTypes = Assembly.GetExecutingAssembly()
		.GetTypes()
		.Where(t => t.Namespace == "Core.Messages" && t.IsClass)
		.ToList();

	foreach (var type in requestTypes)
	{
		x.AddRequestClient(type);
	}

	x.UsingRabbitMq((ctx, conf) =>
	{
		conf.UseSendFilter(typeof(ValidationFilter<>), ctx);

		conf.ConfigureEndpoints(ctx);

		conf.Host("rabbitmq://rabbit", h =>
		{
			h.Username("guest");
			h.Password("guest");
		});
	});
});

Installer.InstallAuth(builder.Services);

Installer.InstallDataProtection(builder.Services, builder.Environment.IsDevelopment());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}

Installer.EnableCommonServices(app);
app.UseCors("AllowFrontend");

app.Run();

public partial class Program {}