using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SalesMicro.Data
{
	public class SaleMapping : IEntityTypeConfiguration<Sale>
	{
		public void Configure(EntityTypeBuilder<Sale> builder)
		{
			builder.HasMany(x => x.SubProducts).WithOne(x => x.Sale).HasForeignKey(x => x.SaleId);
			builder.HasMany(x => x.SaleParameters).WithOne(x => x.Sale).HasForeignKey(x => x.SaleId);
			builder.HasOne(x => x.Seller).WithMany().HasForeignKey(x => x.SellerId);
			builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId);
			builder.HasOne(x => x.Product).WithMany(x => x.Sales).HasForeignKey(x => x.ProductId);
		}
	}

	public class SubProductInSaleMapping : IEntityTypeConfiguration<SubProductInSale>
	{
		public void Configure(EntityTypeBuilder<SubProductInSale> builder)
		{
			builder.HasKey(x => new { x.SaleId, x.SubProductId });
			builder.HasOne(x => x.SubProduct).WithMany(x => x.SubProductInSales).HasForeignKey(x => x.SubProductId);
		}
	}
	
	public class SaleParameterMapping : IEntityTypeConfiguration<SaleParameter>
	{
		public void Configure(EntityTypeBuilder<SaleParameter> builder)
		{
			builder.HasKey(x => new { x.SaleId, x.ParameterId });
			builder.HasOne(x => x.Parameter).WithMany(x => x.SaleParameters).HasForeignKey(x => x.ParameterId);
			builder.HasOne(x => x.Option).WithMany().HasForeignKey(x => x.OptionId).OnDelete(DeleteBehavior.SetNull);
		}
	}
}