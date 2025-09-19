using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebCommons;
using Validators.Users;

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

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseCors("AllowFrontend");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy(new CookiePolicyOptions(){ Secure = CookieSecurePolicy.None});
app.UseSession();
app.UseHttpsRedirection();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=test}/{action=get}"
);

app.Run();

public partial class Program {}