using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ModularMonolith.Modules.Auth.Authentication;
using ModularMonolith.Modules.Auth.Domain;
using ModularMonolith.Modules.Auth.Persistence;
using ModularMonolith.Shared.Infrastructure.Authentication;
using ModularMonolith.Shared.Infrastructure.Multitenancy;

namespace ModularMonolith.Modules.Auth.Features;

public sealed record LoginCommand(string Email, string Password);

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

public static class LoginHandler
{
    public static async Task<AuthTokenResponse> Handle(
        LoginCommand command,
        AuthCatalogDbContext catalog,
        ILookupNormalizer normalizer,
        ITenantScopeFactory tenantScopeFactory,
        IJwtTokenIssuer tokenIssuer,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = normalizer.NormalizeEmail(command.Email);

        var entry = await catalog.TenantUserIndex
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);

        if (entry is null)
        {
            throw new AuthenticationFailedException();
        }

        await using var tenantScope = tenantScopeFactory.CreateScope(entry.TenantId);

        var users = tenantScope.Services.GetRequiredService<UserManager<AppUser>>();
        var user = await users.FindByIdAsync(entry.UserId.ToString());

        if (user is null || !await users.CheckPasswordAsync(user, command.Password))
        {
            throw new AuthenticationFailedException();
        }

        return new AuthTokenResponse(tokenIssuer.Issue(user.Id, entry.TenantId, user.Email));
    }
}
