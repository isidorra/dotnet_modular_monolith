namespace ModularMonolith.Shared.Infrastructure.Provisioning;

public interface ITenantProvisioner
{
    Task<TenantProvisioningSession> BeginAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
