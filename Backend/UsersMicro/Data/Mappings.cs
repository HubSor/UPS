using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UsersMicro.Data
{
	public class UserMapping : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasMany(x => x.Roles).WithMany().UsingEntity("UserRoles");
		}
	}

	public class RoleMapping : IEntityTypeConfiguration<Role>
	{
		public void Configure(EntityTypeBuilder<Role> builder)
		{
			builder.Property(x => x.Id).HasConversion<int>().IsRequired();
		}
	}
}