using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace SalesMicro.Data
{
	public class SalesDataInitializer : BaseDataInitializer
	{
		public static void Initialize(SalesUnitOfWork context)
		{
			if (context.Database.IsSqlite()) return;

			if (context.Database.GetPendingMigrations().Any())
				context.Database.Migrate();

			context.SaveChanges();
			Clear(context);

			context.Sales.Add(new()
			{
				ProductId = 1,
				FinalPrice = 100.99m,
				ProductPrice = 99.99m,
				ProductTax = 7.25m,
				SellerId = 1,
				ClientId = 1,
				SaleTime = DateTime.Now,
				ProductCode = "POSOB",
				SubProductCodes = "",
				ClientName = "Jan Kowalski",
			});

			context.SaveChanges();
		}
	}
}
