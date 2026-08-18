namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public interface ITenantContext
{
    Guid TenantId { get; }
}
