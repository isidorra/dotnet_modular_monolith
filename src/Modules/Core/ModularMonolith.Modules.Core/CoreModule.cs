using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddScoped<ITenantProvisioningParticipant, EmployeeProvisioningParticipant>();
    }
}