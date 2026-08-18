using System.Data.Common;

using Microsoft.Extensions.DependencyInjection;

namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public sealed class TenantScopeFactory : ITenantScopeFactory
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TenantScopeFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public ITenantScope CreateScope(Guid tenantId, DbConnection connection = null, DbTransaction transaction = null)
    {
        var scope = _scopeFactory.CreateAsyncScope();

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

    private sealed class TenantScope : ITenantScope
    {
        private readonly AsyncServiceScope _scope;

        public TenantScope(Guid tenantId, AsyncServiceScope scope)
        {
            TenantId = tenantId;
            _scope = scope;
        }

        public Guid TenantId { get; }

        public IServiceProvider Services => _scope.ServiceProvider;

        public ValueTask DisposeAsync() => _scope.DisposeAsync();
    }
}
