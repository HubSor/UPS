using Microsoft.EntityFrameworkCore;

namespace Data
{
	public abstract class BaseDataInitializer
	{
		protected static void Clear(BaseUnitOfWork context)
		{
			context.Model.GetEntityTypes()
				.Select(t => t.GetTableName())
				.Distinct()
				.ToList()
				.ForEach(tableName =>
				{
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
                    context.Database.ExecuteSqlRaw($"TRUNCATE \"{tableName}\" RESTART IDENTITY CASCADE;");
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
                });

			context.SaveChanges();
		}
	}
}
