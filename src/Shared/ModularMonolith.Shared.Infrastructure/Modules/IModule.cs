using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ModularMonolith.Shared.Infrastructure.Modules;

public interface IModule
{
    string Name { get; }

    Assembly Assembly => GetType().Assembly;

    void AddModule(IServiceCollection services, IConfiguration configuration);
}
