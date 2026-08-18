using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ModularMonolith.Shared.Infrastructure.Modules;

namespace ModularMonolith.Modules.Notifications;

public sealed class NotificationsModule : IModule
{
    public string Name => "Notifications";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {

    }
}
