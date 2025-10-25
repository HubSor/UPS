using System.Reflection;
using Core.Data;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Web;

public static class Installer
{
    public static void InstallCommonMicroServices<TUnitOfWork>(WebApplicationBuilder builder) where TUnitOfWork : DbContext, IUnitOfWork
    {
        InstallDbContext<TUnitOfWork>(builder);

        builder.Services.AddControllersWithViews(x => x.Filters.Add<ExceptionFilter>());

        builder.Services.AddScoped<IUnitOfWork, TUnitOfWork>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddSwaggerGen();
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(TUnitOfWork));

        InstallMassTransit(builder.Services);

        InstallAuth(builder.Services);

        InstallDataProtection(builder.Services, builder.Environment.IsDevelopment());
    }

    private static void InstallDbContext<TUnitOfWork>(WebApplicationBuilder builder) where TUnitOfWork : DbContext, IUnitOfWork
    {
        try
        {
            builder.Services.AddDbContext<TUnitOfWork>(options =>
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("Connection"),
                    op =>
                    {
                        op.MigrationsAssembly(typeof(TUnitOfWork).Assembly.FullName);
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
    }

    public static void InstallDataProtection(IServiceCollection services, bool isDev)
    {
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(isDev ? "/home/hubert/studia/UPS/protection" : "/app/keys"))
            .SetApplicationName("UPS");
    }

    public static void InstallAuth(IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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
                conf.Cookie.Name = "UPSAuth";
                conf.Cookie.SameSite = SameSiteMode.Strict;
            });
        services.AddAuthorization();
        services.AddSession();
    }

    public static void InstallMassTransit(IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            var assembly = Assembly.GetEntryAssembly();
            x.AddConsumers(assembly);

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

                conf.Host("rabbitmq://localhost", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        });
    }

    public static void EnableCommonServices(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseHttpsRedirection();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=test}/{action=get}"
        );

        EnableAuth(app);
    }

    public static void EnableAuth(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
    }
}