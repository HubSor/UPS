using Consumers.Products;
using Core;
using Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UPSContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("UPS_Connection"),
   op => op.MigrationsAssembly(typeof(UPSContext).Assembly.FullName))
);

builder.Services.AddControllers();
builder.Services.AddCors(op =>
{
    op.AddPolicy("AllowFrontend", pol =>
    {
        //pol.WithOrigins(builder.Configuration.GetValue<string>("FRONTEND_ORIGIN") ?? "http://localhost:3000")
        pol.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddMediator(op =>
{
    op.AddConsumersFromNamespaceContaining<AddProductConsumer>();
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UPSContext>();
    DataInitializer.Initialize(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=test}/{action=get}"
);

app.Run();