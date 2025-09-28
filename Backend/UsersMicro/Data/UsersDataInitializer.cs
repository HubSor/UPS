using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using UsersMicro.Models;
using UsersMicro.Services;

namespace UsersMicro.Data
{
	public class UsersDataInitializer : BaseDataInitializer
	{
		public static void InitializeTest(UsersUnitOfWork context, IPasswordService passwordService)
		{
			if (context.Database.GetPendingMigrations().Any())
				context.Database.Migrate();

			var roles = new List<Role>()
			{
				new (){ Id = RoleEnum.Administrator, Description = "Pełne prawa do aplikacji, może wszystko.", Name = "Administrator" },
				new (){ Id = RoleEnum.Seller, Description = "Prawa do sprzedaży na ścieżce sprzedaży.", Name = "Sprzedawca" },
				new (){ Id = RoleEnum.UserManager, Description = "Prawa do zarządzania kontami użytkowników.", Name = "Zarządca użytkowników" },
				new (){ Id = RoleEnum.ProductManager, Description = "Prawa do zarządzania produktami i podproduktami.", Name = "Zarządca produktów" },
				new (){ Id = RoleEnum.SaleManager, Description = "Prawo do wyświetlania historii transakcji.", Name = "Zarządca sprzedaży" },
				new (){ Id = RoleEnum.ClientManager, Description = "Prawo do zarządzania klientami.", Name = "Zarządca klientów" },
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

			context.SaveChanges();
		}

		public static void Initialize(UsersUnitOfWork context, IPasswordService passwordService)
		{
			if (context.Database.IsSqlite()) return;

			if (context.Database.GetPendingMigrations().Any())
				context.Database.Migrate();

			context.SaveChanges();
			Clear(context);

			var roles = new List<Role>()
			{
				new (){ Id = RoleEnum.Administrator, Description = "Pełne prawa do aplikacji, może wszystko.", Name = "Administrator" },
				new (){ Id = RoleEnum.Seller, Description = "Prawa do sprzedaży na ścieżce sprzedaży.", Name = "Sprzedawca" },
				new (){ Id = RoleEnum.UserManager, Description = "Prawa do zarządzania kontami użytkowników.", Name = "Zarządca użytkowników" },
				new (){ Id = RoleEnum.ProductManager, Description = "Prawa do zarządzania produktami i podproduktami.", Name = "Zarządca produktów" },
				new (){ Id = RoleEnum.SaleManager, Description = "Prawo do wyświetlania historii transakcji.", Name = "Zarządca sprzedaży" },
				new (){ Id = RoleEnum.ClientManager, Description = "Prawo do zarządzania klientami.", Name = "Zarządca klientów" },
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

			context.AddressTypes.AddRange(new List<AddressType>()
			{
				new (){ Id = AddressTypeEnum.Residence, Name = "Adres zamieszkania"},
				new (){ Id = AddressTypeEnum.Correspondence, Name = "Adres korespondencyjny"},
				new (){ Id = AddressTypeEnum.Registered, Name = "Adres zameldowania"},
			});
		}
	}
}
