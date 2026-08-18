using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ModularMonolith.Modules.Core.Persistence;
using ModularMonolith.Shared.Infrastructure.Modules;
using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Core;

public sealed class CoreModule : IModule
{
    public string Name => "Core";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<CoreDbContext>(configuration, "core");
    }
}
