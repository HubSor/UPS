using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Services;

namespace Data
{
	public static class DataInitializer
	{
		public static void Initialize(UnitOfWork context, IPasswordService passwordService)
		{
			//context.Database.EnsureCreated();
						
			if (context.Database.GetPendingMigrations().Any())
			{
				context.Database.Migrate();
			}
			
			Clear(context);
					
			var roles = new List<Role>()
			{
				new (){ Id = RoleEnum.Administrator, Description = "Pełne prawa do aplikacji, może wszystko." },
				new (){ Id = RoleEnum.Seller, Description = null },
				new (){ Id = RoleEnum.UserManager, Description = "Prawa do zarządzania kontami użytkowników." },
			};
			
			context.Roles.AddRange(roles);
			
			var salt = passwordService.GenerateSalt();
			var hash = passwordService.GenerateHash("admin", salt);
			
			context.Users.Add(new User()
			{ 
				Name = "admin",
				Active = true,
				Roles = roles,
				Salt = salt,
				Hash = hash,
			});
			
			context.Users.Add(new User()
			{
				Name = "test",
				Active = true,
				Roles = roles.Where(r => r.Id == RoleEnum.Seller).ToList(),
				Salt = salt,
				Hash = hash
			});
			
			context.ProductStatuses.AddRange(new List<ProductStatus>()
			{
				new (){ Id = ProductStatusEnum.NotOffered, Description = "Sprzedaż tego produktu nie będzie możliwa."},
				new (){ Id = ProductStatusEnum.Offered, Description = "Sprzedaż tego produktu jest dozwolona"},
				new (){ Id = ProductStatusEnum.Withdrawn, Description = "Produkt wycofany ze sprzedaży. Sprzedaż niemożliwa."},
			});
				
			context.Products.Add(new()
			{
				Name = "Produkt testowy",
				Code = "TEST1",
				Status = ProductStatusEnum.Offered,
				AnonymousSaleAllowed = false,
				Description = "Testowy produkt",
				Price = 99.99m
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
