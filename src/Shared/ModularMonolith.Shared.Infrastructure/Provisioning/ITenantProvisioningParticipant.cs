namespace ModularMonolith.Shared.Infrastructure.Provisioning;

public interface ITenantProvisioningParticipant
{
    Task OnTenantProvisionedAsync(
        TenantProvisioningSession session,
        TenantRegistration registration,
        CancellationToken cancellationToken);
}
