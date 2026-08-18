using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ModularMonolith.Shared.Infrastructure.Multitenancy;

using Npgsql;

namespace ModularMonolith.Shared.Infrastructure.Provisioning;

public sealed class TenantProvisioningSession : IAsyncDisposable
{
    private readonly NpgsqlConnection _connection;
    private readonly NpgsqlTransaction _transaction;
    private readonly ITenantScope _scope;
    private readonly HashSet<Type> _enlisted = [];

    private bool _committed;

    internal TenantProvisioningSession(
        Guid tenantId,
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        ITenantScope scope)
    {
        TenantId = tenantId;
        _connection = connection;
        _transaction = transaction;
        _scope = scope;
    }

    public Guid TenantId { get; }

    public IServiceProvider TenantServices => _scope.Services;

    public TContext UseContext<TContext>()
        where TContext : DbContext
    {
        var context = _scope.Services.GetRequiredService<TContext>();

        if (_enlisted.Add(typeof(TContext)))
        {
            context.Database.UseTransaction(_transaction);
        }

        return context;
    }

    public Task SetSearchPathAsync(string schema, CancellationToken cancellationToken = default)
    {
        return TenantProvisioner.ExecuteAsync(
            _connection,
            _transaction,
            $"SET LOCAL search_path = {TenantSchema.Quote(schema)};",
            cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
        _committed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_committed)
        {
            await _transaction.RollbackAsync();
        }

        await _scope.DisposeAsync();
        await _transaction.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
