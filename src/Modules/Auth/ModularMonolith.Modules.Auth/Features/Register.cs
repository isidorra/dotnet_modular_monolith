using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ModularMonolith.Modules.Auth.Authentication;
using ModularMonolith.Modules.Auth.Domain;
using ModularMonolith.Modules.Auth.Persistence;
using ModularMonolith.Shared.Infrastructure.Multitenancy;
using ModularMonolith.Shared.Infrastructure.Provisioning;

namespace ModularMonolith.Modules.Auth.Features;

public sealed record RegisterCommand(string TenantName, string Email, string Password);

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(AuthCatalogDbContext catalog, ILookupNormalizer normalizer)
    {
        RuleFor(x => x.TenantName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256)
            .MustAsync(async (email, cancellationToken) =>
            {
                var normalizedEmail = normalizer.NormalizeEmail(email);

                return !await catalog.TenantUserIndex
                    .AnyAsync(entry => entry.NormalizedEmail == normalizedEmail, cancellationToken);
            })
            .WithMessage("An account with this email address already exists");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(PasswordPolicy.RequiredLength)
            .Matches("[A-Z]").WithMessage("'{PropertyName}' must contain an uppercase letter")
            .Matches("[a-z]").WithMessage("'{PropertyName}' must contain a lowercase letter")
            .Matches("[0-9]").WithMessage("'{PropertyName}' must contain a digit")
            .Matches("[^a-zA-Z0-9]").WithMessage("'{PropertyName}' must contain a non-alphanumeric character");
    }
}

public static class RegisterHandler
{
    public static async Task<AuthTokenResponse> Handle(
        RegisterCommand command,
        ITenantProvisioner provisioner,
        IJwtTokenIssuer tokenIssuer,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var tenantId = Guid.CreateVersion7();
        var now = timeProvider.GetUtcNow();

        await using var session = await provisioner.BeginAsync(tenantId, cancellationToken);

        session.UseContext<AuthDbContext>();
        await session.SetSearchPathAsync(TenantSchema.For(AuthModule.SchemaPrefix, tenantId), cancellationToken);

        var user = new AppUser
        {
            Id = Guid.CreateVersion7(),
            UserName = command.Email,
            Email = command.Email,
            CreatedAt = now
        };

        var users = session.TenantServices.GetRequiredService<UserManager<AppUser>>();
        var result = await users.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Identity rejected the user: {string.Join("; ", result.Errors.Select(error => error.Description))}");
        }

        var catalog = session.UseContext<AuthCatalogDbContext>();

        catalog.Tenants.Add(new Tenant
        {
            Id = tenantId,
            Name = command.TenantName,
            CreatedAt = now
        });

        catalog.TenantUserIndex.Add(new TenantUserIndexEntry
        {
            NormalizedEmail = user.NormalizedEmail,
            TenantId = tenantId,
            UserId = user.Id,
            CreatedAt = now
        });

        await catalog.SaveChangesAsync(cancellationToken);
        await session.CommitAsync(cancellationToken);

        return new AuthTokenResponse(tokenIssuer.Issue(user.Id, tenantId, user.Email));
    }
}
