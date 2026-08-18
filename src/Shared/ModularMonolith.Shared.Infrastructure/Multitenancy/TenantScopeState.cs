using System.Data.Common;

namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public sealed class TenantScopeState
{
    public Guid? TenantId { get; private set; }

    public DbConnection Connection { get; private set; }

    public DbTransaction Transaction { get; private set; }

    public void Bind(Guid tenantId, DbConnection connection, DbTransaction transaction)
    {
        TenantId = tenantId;
        Connection = connection;
        Transaction = transaction;
    }
}
