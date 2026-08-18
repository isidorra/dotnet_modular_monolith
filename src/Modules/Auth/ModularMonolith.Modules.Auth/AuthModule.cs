using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ModularMonolith.Modules.Auth.Persistence;
using ModularMonolith.Shared.Infrastructure.Modules;
using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Auth;

public sealed class AuthModule : IModule
{
    public string Name => "Auth";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<AuthDbContext>(configuration, "auth");
    }
}
