using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientsMicro.Models
{	
	public class ClientMapping : IEntityTypeConfiguration<Client>
	{
		public void Configure(EntityTypeBuilder<Client> builder)
		{
			builder.HasMany(x => x.Addresses).WithOne(x => x.Client).HasForeignKey(x => x.ClientId);
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