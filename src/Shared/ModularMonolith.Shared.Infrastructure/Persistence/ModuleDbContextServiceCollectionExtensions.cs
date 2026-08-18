using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ModularMonolith.Shared.Infrastructure.Multitenancy;

using Npgsql;

namespace ModularMonolith.Shared.Infrastructure.Persistence;

public static class ModuleDbContextServiceCollectionExtensions
{
    public static IServiceCollection AddModuleDbContext<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string schemaPrefix)
        where TContext : DbContext
    {
        services.TryAddScoped<ITenantContext, DefaultTenantContext>();

        services.AddDbContext<TContext>((sp, options) =>
        {
            var tenantContext = sp.GetRequiredService<ITenantContext>();
            var baseConnectionString = configuration.GetConnectionString("Postgres")
                ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured");

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString)
            {
                SearchPath = TenantSchema.For(schemaPrefix, tenantContext.TenantId)
            };

            options.UseNpgsql(connectionStringBuilder.ConnectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory");
            });
        });

        return services;
    }
}
