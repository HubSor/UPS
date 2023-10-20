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
			
			context.SaveChanges();
			
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
				BasePrice = 99.99m
			});
			
			context.SubProducts.Add(new() 
			{
				Name = "Podprodukt testowy 1",
				Description = "Testowy podprodukt 1",
				BasePrice = 15.99m,
			});
			
			context.SaveChanges();
			
			context.SubProductsInProducts.Add(new()
			{
				ProductId = 1,
				SubProductId = 1,
				InProductPrice = 9.99m
			});
			
			var parameterTypes = new List<ParameterType>()
			{
				new (){ Id = ParameterTypeEnum.Text, Name="Tekst" },
				new (){ Id = ParameterTypeEnum.Integer, Name="Liczba całkowita" },
				new (){ Id = ParameterTypeEnum.Select, Name="Wybór z listy" },
				new (){ Id = ParameterTypeEnum.TextArea, Name="Pole tekstowe" },
				new (){ Id = ParameterTypeEnum.Decimal, Name="Liczba dziesiętna" },
				new (){ Id = ParameterTypeEnum.Checkbox, Name="Flaga" },
			};
			
			context.ParameterTypes.AddRange(parameterTypes);
			
			context.Parameters.Add(new()
			{
				Name = "Imię zwierzęcia domowego",
				Type = ParameterTypeEnum.Text,
				IsRequired = false,
				ProductId = 1
			});
			
			context.Parameters.Add(new()
			{
				Name = "Dzień tygodnia",
				Type = ParameterTypeEnum.Select,
				IsRequired = true,
				SubProductId = 1
			});
			
			context.SaveChanges();
			
			var days = new List<string>()
			{
				"Poniedziałek",
				"Wtorek",
				"Środa",
				"Czwartek",
				"Piątek",
				"Sobota",
				"Niedziela"
			};
			
			context.ParameterOptions.AddRange(days.Select(d => new ParameterOption() 
			{
				ParameterId = 1,
				Value = d,
			}));

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
					context.Database.ExecuteSqlRaw($"TRUNCATE \"{tableName}\" RESTART IDENTITY CASCADE;");					
				});
				
			context.SaveChanges();
		}
	}
}
