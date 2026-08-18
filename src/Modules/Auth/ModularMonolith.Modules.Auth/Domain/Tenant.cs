namespace ModularMonolith.Modules.Auth.Domain;

public sealed class Tenant
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
