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

            if (!context.Users.Any())
                context.Users.Add(new User()
                { 
                    Id = 1,
                    Name = "Admin" 
                });

            context.SaveChanges();
        }
    }
}
