using System.Data.Common;

using Microsoft.Extensions.DependencyInjection;

namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public sealed class TenantScopeFactory(IServiceScopeFactory scopeFactory) : ITenantScopeFactory
{
    public ITenantScope CreateScope(Guid tenantId, DbConnection connection = null, DbTransaction transaction = null)
    {
        var scope = scopeFactory.CreateAsyncScope();

        try
        {
            scope.ServiceProvider.GetRequiredService<TenantScopeState>().Bind(tenantId, connection, transaction);

            return new TenantScope(tenantId, scope);
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    private sealed class TenantScope(Guid tenantId, AsyncServiceScope scope) : ITenantScope
    {
        public Guid TenantId { get; } = tenantId;

        public IServiceProvider Services => scope.ServiceProvider;

        public ValueTask DisposeAsync()
        {
            return scope.DisposeAsync();
        }
    }
}
