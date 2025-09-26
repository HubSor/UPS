using Data;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Services;

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

			context.Clients.Add(new PersonClient()
			{
				FirstName = "Jan",
				LastName = "Kowalski",
				PhoneNumber = "123456789",
				Email = "test@gmail.com",
				Pesel = "17211116123",
			});

			context.Clients.Add(new PersonClient()
			{
				FirstName = "Janusz",
				LastName = "Nowak",
				PhoneNumber = "999111333",
				Email = "jnowak@gmail.com",
			});

			context.SaveChanges();

			context.Clients.Add(new CompanyClient()
			{
				CompanyName = "krzak sp. z o.o.",
				PhoneNumber = "123456789",
				Email = "test@gmail.com",
				Regon = "133632926",
			});

			context.Clients.Add(new CompanyClient()
			{
				CompanyName = "januszex sp. z o.o.",
				PhoneNumber = "888777666",
				Email = "januszex@gmail.com",
				Nip = "1336329260",
			});

			context.SaveChanges();
		}
	}
}
