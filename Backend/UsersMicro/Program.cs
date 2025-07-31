using Core;
using Data;
using FluentValidation;
using MassTransit;
using Services;
using WebCommons;
using Validators.Users;
using Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(x => x.Filters.Add<ExceptionFilter>());

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); //specific uow
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(PasswordValidator));

builder.Services.AddMediator(mrc => 
{
	mrc.ConfigureMediator((context, cfg) => 
	{
		cfg.UseSendFilter(typeof(ValidationFilter<>), context);
	});
	
	mrc.AddConsumers(typeof(BaseConsumer<,>).Assembly);
});

var app = builder.Build();

try 
{
	using var scope = app.Services.CreateScope();
	{
		var context = scope.ServiceProvider.GetRequiredService<UnitOfWork>(); // specific initializer
		var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
		DataInitializer.Initialize(context, passwordService);
	}
}
catch (Exception)
{
	Console.WriteLine("Database initialization error");
	throw;
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=test}/{action=get}"
);

app.Run();

public partial class Program {}