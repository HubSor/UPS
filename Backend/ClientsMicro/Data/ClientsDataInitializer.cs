using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientsMicro.Data
{
	public class ClientsDataInitializer : BaseDataInitializer
	{
		public static void Initialize(ClientsUnitOfWork context)
		{
			if (context.Database.IsSqlite()) return;

			if (context.Database.GetPendingMigrations().Any())
				context.Database.Migrate();

			context.SaveChanges();
			Clear(context);

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

			context.Clients.Add(new PersonClient()
			{
				FirstName = "Test",
				LastName = "Prywatny",
			});

			context.Clients.Add(new PersonClient()
			{
				FirstName = "Test",
				LastName = "Prywatny2",
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

			context.Clients.Add(new CompanyClient()
			{
				CompanyName = "Test Firma",
			});

			context.Clients.Add(new CompanyClient()
			{
				CompanyName = "Test Firma2",
			});

			context.SaveChanges();
		}
	}
}
