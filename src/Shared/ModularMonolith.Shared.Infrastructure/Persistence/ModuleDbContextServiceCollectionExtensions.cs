using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ModularMonolith.Shared.Infrastructure.Multitenancy;

using Npgsql;

namespace ModularMonolith.Shared.Infrastructure.Persistence;

public static class ModuleDbContextServiceCollectionExtensions
{
    public const string MigrationsHistoryTable = "__ef_migrations_history";

    public static IServiceCollection AddModuleDbContext<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string schemaPrefix)
        where TContext : DbContext
    {
        services.AddMultitenancy();
        services.AddSingleton(new TenantSchemaDescriptor(schemaPrefix, typeof(TContext)));

        services.AddDbContext<TContext>((sp, options) =>
        {
            var scopeState = sp.GetRequiredService<TenantScopeState>();

            if (scopeState.Connection is not null)
            {
                UseSharedConnection(options, scopeState.Connection, null);
                return;
            }

            var tenantContext = sp.GetRequiredService<ITenantContext>();
            var schema = TenantSchema.For(schemaPrefix, tenantContext.TenantId);

            UseSearchPath(options, BaseConnectionString(configuration), schema, null);
        });

        return services;
    }

    public static IServiceCollection AddGlobalDbContext<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string schema)
        where TContext : DbContext
    {
        services.AddMultitenancy();

        services.AddDbContext<TContext>((sp, options) =>
        {
            var scopeState = sp.GetRequiredService<TenantScopeState>();

            if (scopeState.Connection is not null)
            {
                UseSharedConnection(options, scopeState.Connection, schema);
                return;
            }

            UseSearchPath(options, BaseConnectionString(configuration), schema, schema);
        });

        return services;
    }

    public static string BaseConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured");
    }

    public static void UseSearchPath(
        DbContextOptionsBuilder options,
        string baseConnectionString,
        string searchPath,
        string migrationsHistorySchema)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            SearchPath = TenantSchema.Quote(searchPath)
        };

        options.UseNpgsql(connectionStringBuilder.ConnectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable(MigrationsHistoryTable, migrationsHistorySchema);
        });

        options.UseSnakeCaseNamingConvention();
    }

    private static void UseSharedConnection(
        DbContextOptionsBuilder options,
        System.Data.Common.DbConnection connection,
        string migrationsHistorySchema)
    {
        options.UseNpgsql(connection, contextOwnsConnection: false, npgsql =>
        {
            npgsql.MigrationsHistoryTable(MigrationsHistoryTable, migrationsHistorySchema);
        });

        options.UseSnakeCaseNamingConvention();
    }
}
