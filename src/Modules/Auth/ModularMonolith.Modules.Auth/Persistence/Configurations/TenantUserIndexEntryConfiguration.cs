using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModularMonolith.Modules.Auth.Domain;

namespace ModularMonolith.Modules.Auth.Persistence.Configurations;

public sealed class TenantUserIndexEntryConfiguration
    : IEntityTypeConfiguration<TenantUserIndexEntry>, ICatalogEntityConfiguration
{
    public void Configure(EntityTypeBuilder<TenantUserIndexEntry> builder)
    {
        builder.ToTable("tenant_user_index");

        builder.HasKey(x => x.NormalizedEmail);

        builder.Property(x => x.NormalizedEmail).HasMaxLength(256);
        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.TenantId);
    }
}
