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
	
	public class ProductMapping : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.HasMany(x => x.SubProductInProducts).WithOne(x => x.Product).HasForeignKey(x => x.ProductId);
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