namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public static class TenantSchema
{
    public static string For(string modulePrefix, Guid tenantId) => $"{modulePrefix}_{tenantId:N}";
}
