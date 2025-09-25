using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebCommons;

public static class Installer
{
    public static void InstallDataProtection(IServiceCollection services)
    {
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/home/hubert/studia/UPS/protection"))
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

    public static void EnableAuth(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
    }
}