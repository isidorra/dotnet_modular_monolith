using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModularMonolith.Modules.Auth.Domain;

namespace ModularMonolith.Modules.Auth.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>, ICatalogEntityConfiguration
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
