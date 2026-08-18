using System.Data.Common;

namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public interface ITenantScope : IAsyncDisposable
{
    Guid TenantId { get; }

    IServiceProvider Services { get; }
}

public interface ITenantScopeFactory
{
    ITenantScope CreateScope(Guid tenantId, DbConnection connection = null, DbTransaction transaction = null);
}
