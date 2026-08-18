using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

using ModularMonolith.Modules.Auth.Features;

using Wolverine;

namespace ModularMonolith.Modules.Auth;

internal static class AuthEndpoints
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        var auth = endpoints.MapGroup("/auth").AllowAnonymous();

        auth.MapPost("/register", (RegisterCommand command, IMessageBus bus, CancellationToken cancellationToken) =>
        {
            return bus.InvokeAsync<AuthTokenResponse>(command, cancellationToken);
        });

        auth.MapPost("/login", (LoginCommand command, IMessageBus bus, CancellationToken cancellationToken) =>
        {
            return bus.InvokeAsync<AuthTokenResponse>(command, cancellationToken);
        });
    }
}
