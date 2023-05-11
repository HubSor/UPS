using Consumers.Products;
using Core;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddDbContext<UPSContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("UPS_Connection")));
builder.Services.AddControllers();
builder.Services.AddCors(op =>
{
    op.AddPolicy("AllowFrontend", pol =>
    {
        pol.WithOrigins(builder.Configuration.GetValue<string>("FRONTEND_ORIGIN") ?? "http://localhost:3000")
            .AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddMassTransit();
builder.Services.AddMediator(op =>
{
    op.AddConsumersFromNamespaceContaining(typeof(AddProductConsumer));
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<UPSContext>();
        var created = context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=test}/{action=get}"
);

app.Run();