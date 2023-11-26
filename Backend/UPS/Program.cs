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
				op.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
			});
	});
}
catch (Exception)
{
	Console.WriteLine("Database connection error");
	throw;
}

builder.Services.AddControllersWithViews(x => x.Filters.Add(typeof(ExceptionFilter)));
builder.Services.AddCors(op =>
{
	op.AddPolicy("AllowFrontend", pol =>
	{
		pol.WithOrigins(builder.Configuration.GetValue<string>("FRONTEND_ORIGIN") ?? "", "https://localhost:3000", "http://localhost:3000")
			.AllowAnyMethod().AllowAnyHeader().AllowCredentials();
		pol.SetPreflightMaxAge(TimeSpan.FromMinutes(15));
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IPasswordService), typeof(PasswordService));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(PasswordValidator));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(conf => 
	{
		conf.ExpireTimeSpan = TimeSpan.FromMinutes(30);
		conf.SlidingExpiration = true;
		conf.Events.OnRedirectToLogin = context => 
		{
			context.Response.StatusCode = 401;
			return Task.CompletedTask;
		};
		conf.AccessDeniedPath = "/";
		conf.Events.OnRedirectToAccessDenied = context =>
		{
			context.Response.StatusCode = 401;
			return Task.CompletedTask;	
		};
		conf.Cookie.IsEssential = true;
		conf.Cookie.SecurePolicy = CookieSecurePolicy.Always;
		conf.Cookie.HttpOnly = true;
		conf.Cookie.Name="UPSAuth";
		conf.Cookie.SameSite = SameSiteMode.Strict;
	});
builder.Services.AddAuthorization();
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
app.UseCors("AllowFrontend");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy(new CookiePolicyOptions(){ Secure = CookieSecurePolicy.None});
app.UseSession();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=test}/{action=get}"
);

app.Run();