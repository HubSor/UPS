using Core.Data;
using Core.Models;
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

			// todo

			context.SaveChanges();
		}
	}
}
