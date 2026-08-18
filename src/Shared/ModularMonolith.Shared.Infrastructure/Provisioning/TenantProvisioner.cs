using Microsoft.Extensions.Configuration;

using ModularMonolith.Shared.Infrastructure.Multitenancy;
using ModularMonolith.Shared.Infrastructure.Persistence;

using Npgsql;

namespace ModularMonolith.Shared.Infrastructure.Provisioning;

public sealed class TenantProvisioner : ITenantProvisioner
{
    private readonly IConfiguration _configuration;
    private readonly IEnumerable<TenantSchemaDescriptor> _descriptors;
    private readonly TenantMigrationScriptCache _scripts;
    private readonly ITenantScopeFactory _scopeFactory;

    public TenantProvisioner(
        IConfiguration configuration,
        IEnumerable<TenantSchemaDescriptor> descriptors,
        TenantMigrationScriptCache scripts,
        ITenantScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _descriptors = descriptors;
        _scripts = scripts;
        _scopeFactory = scopeFactory;
    }

    public async Task<TenantProvisioningSession> BeginAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(
            ModuleDbContextServiceCollectionExtensions.BaseConnectionString(_configuration));

        await connection.OpenAsync(cancellationToken);

        NpgsqlTransaction transaction = null;

        try
        {
            transaction = await connection.BeginTransactionAsync(cancellationToken);

            foreach (var descriptor in _descriptors)
            {
                var schema = TenantSchema.For(descriptor.SchemaPrefix, tenantId);

                await ExecuteAsync(connection, transaction, $"CREATE SCHEMA {TenantSchema.Quote(schema)};", cancellationToken);
                await ExecuteAsync(connection, transaction, $"SET LOCAL search_path = {TenantSchema.Quote(schema)};", cancellationToken);

                var script = _scripts.ScriptFor(descriptor);

                if (!string.IsNullOrWhiteSpace(script))
                {
                    await ExecuteAsync(connection, transaction, script, cancellationToken);
                }
            }

            return new TenantProvisioningSession(tenantId, connection, transaction, _scopeFactory.CreateScope(tenantId, connection, transaction));
        }
        catch
        {
            if (transaction is not null)
            {
                await transaction.DisposeAsync();
            }

            await connection.DisposeAsync();
            throw;
        }
    }

    internal static async Task ExecuteAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string sql,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();

        command.CommandText = sql;
        command.Transaction = transaction;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
