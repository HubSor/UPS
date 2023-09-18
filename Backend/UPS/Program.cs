using Consumers;
using Core;
using Data;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Services;
using UPS.Filters;
using Validators.Users;

var builder = WebApplication.CreateBuilder(args);

try
{
	builder.Services.AddDbContext<UnitOfWork>(options => 
	{
		options.UseNpgsql(builder.Configuration.GetConnectionString("UPS_Connection"),
			op => {
				op.MigrationsAssembly(typeof(UnitOfWork).Assembly.FullName);
			});
	});
}
catch (Exception ex)
{
	Console.WriteLine("Database connection error");
	Console.Write(ex);
}

builder.Services.AddControllersWithViews(x => x.Filters.Add(typeof(ExceptionFilter)));
builder.Services.AddCors(op =>
{
	op.AddPolicy("AllowFrontend", pol =>
	{
		//pol.WithOrigins(builder.Configuration.GetValue<string>("FRONTEND_ORIGIN") ?? "http://localhost:3000")
		pol.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
	});
});

builder.Services.AddMediator(mrc => 
{
	mrc.ConfigureMediator((context, cfg) => 
	{
		cfg.UseSendFilter(typeof(ValidationFilter<>), context);
	});
	
	mrc.AddConsumers(typeof(BaseConsumer<,>).Assembly);
});

builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IPasswordService), typeof(PasswordService));
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(LoginValidator));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(conf => 
	{
		conf.ExpireTimeSpan = TimeSpan.FromMinutes(15);
		conf.SlidingExpiration = true;
		conf.LoginPath = "/login";
		conf.LogoutPath = "/logout";
	});
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

var app = builder.Build();

try 
{
	using var scope = app.Services.CreateScope();
	{
		var context = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
		var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
		DataInitializer.Initialize(context, passwordService);
	}
}
catch (Exception ex)
{
	Console.WriteLine("Database initialization error");
	Console.Write(ex);
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
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy(new CookiePolicyOptions(){ Secure = CookieSecurePolicy.None});
app.UseCors("AllowFrontend");
app.UseSession();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=test}/{action=get}"
);

app.Run();