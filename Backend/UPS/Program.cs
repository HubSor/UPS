using Core.Messages;
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
	// dodać konsumentów, ale tak żeby było wiadomo gdzie co idzie, chyba że  defaultu tak będzie
	// a może to w mikro?
	x.AddConsumer()

	x.UsingRabbitMq((ctx, conf) =>
	{
		conf.AddPublishMessageTypesFromNamespaceContaining<LoginOrder>();
		conf.UseSendFilter(typeof(ValidationFilter<>), ctx);

		conf.ConfigureEndpoints(ctx);
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