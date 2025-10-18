using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicro.Data
{
	public class ProductsDataInitializer : BaseDataInitializer
	{
		public static void Initialize(ProductsUnitOfWork context)
		{
			if (context.Database.IsSqlite()) return;

			if (context.Database.GetPendingMigrations().Any())
				context.Database.Migrate();

			context.SaveChanges();
			Clear(context);

			context.ProductStatuses.AddRange(new List<ProductStatus>()
			{
				new (){ Id = ProductStatusEnum.NotOffered, Description = "Sprzedaż tego produktu nie będzie możliwa.", Name = "Nieoferowany" },
				new (){ Id = ProductStatusEnum.Offered, Description = "Sprzedaż tego produktu jest dozwolona", Name = "Oferowany" },
				new (){ Id = ProductStatusEnum.Withdrawn, Description = "Produkt wycofany ze sprzedaży. Sprzedaż niemożliwa.", Name = "Wycofany" },
			});

			context.AddressTypes.AddRange(new List<AddressType>()
			{
				new (){ Id = AddressTypeEnum.Residence, Name = "Adres zamieszkania"},
				new (){ Id = AddressTypeEnum.Correspondence, Name = "Adres korespondencyjny"},
				new (){ Id = AddressTypeEnum.Registered, Name = "Adres zameldowania"},
			});

			context.Products.Add(new()
			{
				Name = "Produkt osobowy",
				Code = "POSOB",
				Status = ProductStatusEnum.Offered,
				AnonymousSaleAllowed = false,
				Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
				BasePrice = 99.99m,
				TaxRate = 0.23m,
			});
			
			context.SubProducts.Add(new() 
			{
				Name = "Podprodukt testowy 1",
				Description = "Testowy podprodukt 1",
				Code = "PP1",
				BasePrice = 15.99m,
				TaxRate = 0.20m,
			});

			context.SubProducts.Add(new()
			{
				Name = "Podprodukt testowy 2",
				Description = "Testowy podprodukt 2",
				Code = "PP2",
				BasePrice = 0.99m,
				TaxRate = 0.01m,
			});

			context.SubProducts.Add(new()
			{
				Name = "Podprodukt testowy 3",
				Description = "Testowy podprodukt 3",
				Code = "PP3",
				BasePrice = 4.99m,
				TaxRate = 0.45m,
			});

			context.SaveChanges();

			context.SubProductsInProducts.Add(new()
			{
				ProductId = 1,
				SubProductId = 1,
				InProductPrice = 9.99m
			});

			context.SubProductsInProducts.Add(new()
			{
				ProductId = 1,
				SubProductId = 2,
				InProductPrice = 9.99m
			});

			context.Products.Add(new()
			{
				Name = "Produkt anonimowy",
				Code = "PANON",
				Status = ProductStatusEnum.Offered,
				AnonymousSaleAllowed = true,
				Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
				BasePrice = 99.99m,
				TaxRate = 0.09m,
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
				Required = false,
				SubProductId = 3
			});

			context.Parameters.Add(new()
			{
				Name = "Wzrost kupującego (m)",
				Type = ParameterTypeEnum.Decimal,
				Required = false,
				ProductId = 1
			});

			context.Parameters.Add(new()
			{
				Name = "Czy masz dobry humor?",
				Type = ParameterTypeEnum.Checkbox,
				Required = false,
				ProductId = 1
			});

			context.Parameters.Add(new()
			{
				Name = "Miasto",
				Type = ParameterTypeEnum.Text,
				Required = true,
				ProductId = 2
			});

			context.Parameters.Add(new()
			{
				Name = "Ile nóg ma człowiek?",
				Type = ParameterTypeEnum.Integer,
				Required = true,
				ProductId = 2
			});

			context.Parameters.Add(new()
			{
				Name = "Ile to jest 5/2?",
				Type = ParameterTypeEnum.Decimal,
				Required = false,
				SubProductId = 1
			});

			context.Parameters.Add(new()
			{
				Name = "Czy jesteś sprzedawcą?",
				Type = ParameterTypeEnum.Checkbox,
				Required = false,
				SubProductId = 2
			});

			context.Parameters.Add(new()
			{
				Name = "Opisz mi swój dzień",
				Type = ParameterTypeEnum.TextArea,
				Required = false,
				SubProductId = 3
			});

			var dayParam = new Parameter()
			{
				Name = "Dzień tygodnia",
				Type = ParameterTypeEnum.Select,
				Required = true,
				ProductId = 2
			};

			context.Parameters.Add(dayParam);
			
			context.SaveChanges();

			context.SubProductsInProducts.Add(new()
			{
				ProductId = 2,
				SubProductId = 1,
				InProductPrice = 9.99m
			});

			context.SubProductsInProducts.Add(new()
			{
				ProductId = 2,
				SubProductId = 3,
				InProductPrice = 9.99m
			});

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

			var dayOptions = days.Select(d => new ParameterOption()
			{
				ParameterId = dayParam.Id,
				Value = d,
			});
			
			context.SaleParameters.Add(new ()
			{
				SaleId = 1,
				ParameterId = dayParam.Id,
				Value = "Środa",
				OptionId = context.ParameterOptions.FirstOrDefault(o => o.Value == "Środa")?.Id
			});

			context.ParameterOptions.AddRange(dayOptions);

			context.SaveChanges();
		}
	}
}
