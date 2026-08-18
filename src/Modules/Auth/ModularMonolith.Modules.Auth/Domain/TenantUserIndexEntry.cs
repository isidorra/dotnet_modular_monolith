namespace ModularMonolith.Modules.Auth.Domain;

public sealed class TenantUserIndexEntry
{
    public string NormalizedEmail { get; set; }

    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
