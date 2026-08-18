using Microsoft.EntityFrameworkCore;

using ModularMonolith.Modules.Auth.Domain;
using ModularMonolith.Modules.Auth.Persistence.Configurations;
using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Auth.Persistence;

public sealed class AuthCatalogDbContext : DbContext
{
    public const string Schema = "auth_catalog";

    public AuthCatalogDbContext(DbContextOptions<AuthCatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<TenantUserIndexEntry> TenantUserIndex => Set<TenantUserIndexEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AuthCatalogDbContext).Assembly,
            typeof(ICatalogEntityConfiguration).IsAssignableFrom);

        modelBuilder.UseSnakeCaseNames();
    }
}
