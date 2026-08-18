using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ModularMonolith.Modules.Core.Persistence;
using ModularMonolith.Modules.Core.Provisioning;
using ModularMonolith.Shared.Infrastructure.Modules;
using ModularMonolith.Shared.Infrastructure.Persistence;
using ModularMonolith.Shared.Infrastructure.Provisioning;

namespace ModularMonolith.Modules.Core;

public sealed class CoreModule : IModule
{
    public const string SchemaPrefix = "core";

    public string Name => "Core";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<CoreDbContext>(configuration, SchemaPrefix);

        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<ITenantProvisioningParticipant, EmployeeProvisioningParticipant>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        CoreEndpoints.Map(endpoints);
    }
}