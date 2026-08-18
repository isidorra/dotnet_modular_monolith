namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public static class TenantSchema
{
    public static string For(string modulePrefix, Guid tenantId)
    {
        return $"{modulePrefix}_{tenantId:D}";
    }

    public static string Quote(string identifier)
    {
        return $"\"{identifier.Replace("\"", "\"\"")}\"";
    }
}
