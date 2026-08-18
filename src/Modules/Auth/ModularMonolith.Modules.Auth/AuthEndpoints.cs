using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using ModularMonolith.Modules.Auth.Features;

using Wolverine;

namespace ModularMonolith.Modules.Auth;

internal static class AuthEndpoints
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        var auth = endpoints.MapGroup("/auth").AllowAnonymous();

        auth.MapPost("/register", async (RegisterCommand command, IMessageBus bus, CancellationToken cancellationToken) =>
            Results.Ok(await bus.InvokeAsync<AuthTokenResponse>(command, cancellationToken)));

        auth.MapPost("/login", async (LoginCommand command, IMessageBus bus, CancellationToken cancellationToken) =>
            Results.Ok(await bus.InvokeAsync<AuthTokenResponse>(command, cancellationToken)));
    }
}
