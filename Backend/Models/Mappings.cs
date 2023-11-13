using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

namespace Models
{
	public class UserMapping : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasMany(x => x.Roles).WithMany().UsingEntity("UserRoles");
		}
	}
	
	public class ParameterMapping : IEntityTypeConfiguration<Parameter>
	{
		public void Configure(EntityTypeBuilder<Parameter> builder)
		{
			builder.HasOne(x => x.Product).WithMany(x => x.Parameters).HasForeignKey(x => x.ProductId);
			builder.HasOne(x => x.SubProduct).WithMany(x => x.Parameters).HasForeignKey(x => x.SubProductId);
			builder.HasMany(x => x.Options).WithOne(x => x.Parameter).HasForeignKey(x => x.ParameterId);
			builder.HasOne(x => x.TypeObject).WithMany().HasForeignKey(x => x.Type);
			// builder.ToTable("Parameters", opt => opt.HasCheckConstraint("CH_Parameters_ProductOrSubProduct", "num_nonnulls(SubProductId, ProductId) = 1"));
		}
	}
	
	public class SaleMapping : IEntityTypeConfiguration<Sale>
	{
		public void Configure(EntityTypeBuilder<Sale> builder)
		{
			builder.HasMany(x => x.SubProducts).WithOne(x => x.Sale).HasForeignKey(x => x.SaleId);
			builder.HasMany(x => x.SaleParameters).WithOne(x => x.Sale).HasForeignKey(x => x.SaleId);
			builder.HasOne(x => x.Seller).WithMany().HasForeignKey(x => x.SellerId);
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
	
	public class ProductMapping : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.HasMany(x => x.SubProductInProducts).WithOne(x => x.Product).HasForeignKey(x => x.ProductId);
			builder.HasOne(x => x.StatusObject).WithMany().HasForeignKey(x => x.Status);
		}
	}
	
	public class SubProductInProductMapping : IEntityTypeConfiguration<SubProductInProduct>
	{
		public void Configure(EntityTypeBuilder<SubProductInProduct> builder)
		{
			builder.HasKey(x => new { x.ProductId, x.SubProductId });
		}
	}
	
	public class SubProductMapping : IEntityTypeConfiguration<SubProduct>
	{
		public void Configure(EntityTypeBuilder<SubProduct> builder)
		{
			builder.HasMany(x => x.SubProductInProducts).WithOne(x => x.SubProduct).HasForeignKey(x => x.SubProductId);
		}
	}
	
	public class ParameterTypeMapping : IEntityTypeConfiguration<ParameterType>
	{
		public void Configure(EntityTypeBuilder<ParameterType> builder)
		{
			builder.Property(x => x.Id).HasConversion<int>().IsRequired();
		}
	}
	
	public class RoleMapping : IEntityTypeConfiguration<Role>
	{
		public void Configure(EntityTypeBuilder<Role> builder)
		{
			builder.Property(x => x.Id).HasConversion<int>().IsRequired();
		}
	}
	
	public class ProductStatusMapping : IEntityTypeConfiguration<ProductStatus>
	{
		public void Configure(EntityTypeBuilder<ProductStatus> builder)
		{
			builder.Property(x => x.Id).HasConversion<int>().IsRequired();
		}
	}
}