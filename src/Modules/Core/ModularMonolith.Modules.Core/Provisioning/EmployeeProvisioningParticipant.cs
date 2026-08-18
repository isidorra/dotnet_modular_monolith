using ModularMonolith.Modules.Core.Domain;
using ModularMonolith.Modules.Core.Persistence;
using ModularMonolith.Shared.Infrastructure.Multitenancy;
using ModularMonolith.Shared.Infrastructure.Provisioning;

namespace ModularMonolith.Modules.Core.Provisioning;

public sealed class EmployeeProvisioningParticipant : ITenantProvisioningParticipant
{
    public async Task OnTenantProvisionedAsync(
        TenantProvisioningSession session,
        TenantRegistration registration,
        CancellationToken cancellationToken)
    {
        var core = session.UseContext<CoreDbContext>();

        await session.SetSearchPathAsync(
            TenantSchema.For(CoreModule.SchemaPrefix, registration.TenantId),
            cancellationToken);

        core.Employees.Add(new Employee
        {
            Id = Guid.CreateVersion7(),
            UserId = registration.UserId,
            FirstName = registration.FirstName,
            LastName = registration.LastName,
            Email = registration.Email,
            CreatedAt = registration.RegisteredAt
        });

        await core.SaveChangesAsync(cancellationToken);
    }
}
