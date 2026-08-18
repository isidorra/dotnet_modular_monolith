namespace ModularMonolith.Shared.Infrastructure.Provisioning;

public sealed record TenantRegistration(
    Guid TenantId,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTimeOffset RegisteredAt);