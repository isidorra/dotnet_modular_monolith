using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ModularMonolith.Modules.Auth.Authentication;
using ModularMonolith.Modules.Auth.Domain;
using ModularMonolith.Modules.Auth.Persistence;
using ModularMonolith.Shared.Infrastructure.Modules;
using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Auth;

public sealed class AuthModule : IModule
{
    public const string SchemaPrefix = "auth";

    public string Name => "Auth";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<AuthDbContext>(configuration, SchemaPrefix);
        services.AddGlobalDbContext<AuthCatalogDbContext>(configuration, AuthCatalogDbContext.Schema);

        services.AddIdentityCore<AppUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedAccount = false;
            PasswordPolicy.Apply(options.Password);
        }).AddEntityFrameworkStores<AuthDbContext>();

        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<IJwtTokenIssuer, JwtTokenIssuer>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AuthEndpoints.Map(endpoints);
    }
}
