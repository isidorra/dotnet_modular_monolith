using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ModularMonolith.Shared.Infrastructure.Modules;

namespace ModularMonolith.Modules.Core;

public sealed class CoreModule : IModule
{
    public string Name => "Core";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {

    }
}
