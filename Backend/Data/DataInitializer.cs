using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Services;

namespace Data
{
	public static class DataInitializer
	{
		public static void Initialize(UnitOfWork context, IPasswordService passwordService)
		{
			
			context.Database.EnsureCreated();
						
			if (context.Database.GetPendingMigrations().Any())
			{
				context.Database.Migrate();
			}
			
			Clear(context);
					
			var roles = new List<RoleEntity>()
			{
				new (){ Id = Role.Administrator, Description = "Może wszystko" },
				new (){ Id = Role.Seller, Description = "Może sprzedawać" },
				new (){ Id = Role.UserManager, Description = "Może zarządzać użytkownikami" },
			};
			
			var salt = passwordService.GenerateSalt();
			var hash = passwordService.GenerateHash("admin", salt);
			
			context.Users.Add(new User()
			{ 
				Id = 1,
				Name = "admin",
				Active = true,
				Roles = roles,
				Salt = salt,
				Hash = hash,
			});

			context.SaveChanges();
		}
		
		public static void Clear(UnitOfWork context)
		{
			context.Model.GetEntityTypes()
				.Select(t => t.GetTableName())
				.Distinct()
				.ToList()
				.ForEach(tableName => 
				{
					context.Database.ExecuteSqlRaw($"TRUNCATE \"{tableName}\" RESTART IDENTITY CASCADE ");					
				});
				
			context.SaveChanges();
		}
	}
}
