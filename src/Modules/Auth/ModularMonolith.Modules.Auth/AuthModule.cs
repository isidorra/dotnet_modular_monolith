using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ModularMonolith.Shared.Infrastructure.Modules;

namespace ModularMonolith.Modules.Auth;

public sealed class AuthModule : IModule
{
    public string Name => "Auth";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {

    }
}
