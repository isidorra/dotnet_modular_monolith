using Microsoft.Extensions.Configuration;

namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public sealed class DefaultTenantContext : ITenantContext
{
    public DefaultTenantContext(IConfiguration configuration)
    {
        var tenantId = configuration["Tenancy:DefaultTenantId"]
            ?? throw new InvalidOperationException("Tenancy:DefaultTenantId is not configured");

        TenantId = Guid.Parse(tenantId);
    }

    public Guid TenantId { get; }
}
