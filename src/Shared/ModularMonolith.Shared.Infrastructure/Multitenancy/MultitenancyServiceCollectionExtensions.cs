using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ModularMonolith.Shared.Infrastructure.Provisioning;

namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public static class MultitenancyServiceCollectionExtensions
{
    public static IServiceCollection AddMultitenancy(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.TryAddScoped<TenantScopeState>();
        services.TryAddScoped<ITenantContext, TenantContext>();
        services.TryAddSingleton<ITenantScopeFactory, TenantScopeFactory>();
        services.TryAddSingleton<TenantMigrationScriptCache>();
        services.TryAddSingleton<ITenantProvisioner, TenantProvisioner>();

        return services;
    }
}
