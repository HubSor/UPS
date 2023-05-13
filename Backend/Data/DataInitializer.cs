using Core;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Data
{
    public static class DataInitializer
    {
        public static void Initialize(UPSContext context)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            if (!context.Products.Any())
                context.Products.Add(new Product() { CreatedAt = new DateTime(2000, 1, 1), Name = "Millennium" });

            context.SaveChanges();
        }
    }
}
