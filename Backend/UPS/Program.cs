using Consumers.Products;
using Core;
using Data;
using MassTransit;
using Messages.Products;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UPSContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("UPS_Connection"),
   op => op.MigrationsAssembly(typeof(UPSContext).Assembly.FullName))
);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddCors(op =>
{
    op.AddPolicy("AllowFrontend", pol =>
    {
        //pol.WithOrigins(builder.Configuration.GetValue<string>("FRONTEND_ORIGIN") ?? "http://localhost:3000")
        pol.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddScoped(typeof(IRepository<Product>), typeof(Repository<Product>));
builder.Services.AddScoped(typeof(IConsumer<ViewProductsOrder>), typeof(ViewProductConsumer));
builder.Services.AddMediator(op =>
{
    op.AddConsumersFromNamespaceContaining<AddProductConsumer>();
});
builder.Services.AddMassTransit(op =>
{
    op.AddConsumersFromNamespaceContaining<AddProductConsumer>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<UPSContext>();
        DataInitializer.Initialize(context);
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