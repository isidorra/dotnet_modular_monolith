using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.JsonWebTokens;

using ModularMonolith.Modules.Auth.Authentication;
using ModularMonolith.Shared.Infrastructure.Authentication;
using ModularMonolith.Shared.Infrastructure.Multitenancy;

namespace ModularMonolith.Modules.Auth.Tests.Authentication;

public sealed class JwtTokenIssuerTests
{
    private static readonly Guid UserId = Guid.Parse("01a01672-7b3c-7a44-9e21-4f8d2c6b0e15");
    private static readonly Guid TenantId = Guid.Parse("01a01672-43e5-7de8-98fb-3597d4e38de1");
    private static readonly DateTimeOffset Now = new(2026, 8, 18, 12, 0, 0, TimeSpan.Zero);

    private const string Email = "ada@example.com";

    private static readonly JwtOptions Settings = new()
    {
        Issuer = "modular-monolith",
        Audience = "modular-monolith-api",
        SigningKey = "a-signing-key-that-is-at-least-32-bytes-long",
        AccessTokenMinutes = 30
    };

    [Fact]
    public void Issues_a_token_carrying_the_user_tenant_and_email()
    {
        var token = Issue();

        token.GetClaim(JwtRegisteredClaimNames.Sub).Value.ShouldBe(UserId.ToString());
        token.GetClaim(JwtRegisteredClaimNames.Email).Value.ShouldBe(Email);
        token.GetClaim(TenantClaimTypes.TenantId).Value.ShouldBe(TenantId.ToString());
    }

    [Fact]
    public void Uses_the_literal_tenant_id_claim_name_that_TenantContext_reads()
    {
        Issue().Claims.ShouldContain(claim => claim.Type == "tenant_id");
    }

    [Fact]
    public void Stamps_the_configured_issuer_and_audience()
    {
        var token = Issue();

        token.Issuer.ShouldBe(Settings.Issuer);
        token.Audiences.ShouldBe([Settings.Audience]);
    }

    [Fact]
    public void Expires_the_configured_number_of_minutes_after_the_current_time()
    {
        var token = Issue();

        token.ValidFrom.ShouldBe(Now.UtcDateTime);
        token.ValidTo.ShouldBe(Now.UtcDateTime.AddMinutes(Settings.AccessTokenMinutes));
    }

    [Fact]
    public void Gives_every_token_a_distinct_identifier()
    {
        var issuer = Create();

        var first = Read(issuer.Issue(UserId, TenantId, Email));
        var second = Read(issuer.Issue(UserId, TenantId, Email));

        first.GetClaim(JwtRegisteredClaimNames.Jti).Value
            .ShouldNotBe(second.GetClaim(JwtRegisteredClaimNames.Jti).Value);
    }

    [Fact]
    public void Issues_a_tenant_claim_that_TenantContext_resolves_back_to_the_same_tenant()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(Issue().Claims));

        var tenantContext = new TenantContext(
            new TenantScopeState(),
            new StubHttpContextAccessor(new DefaultHttpContext { User = principal }));

        tenantContext.TenantId.ShouldBe(TenantId);
    }

    private static JsonWebToken Issue()
    {
        return Read(Create().Issue(UserId, TenantId, Email));
    }

    private static JwtTokenIssuer Create()
    {
        return new JwtTokenIssuer(Options.Create(Settings), new FakeTimeProvider(Now));
    }

    private static JsonWebToken Read(string token)
    {
        return new JsonWebTokenHandler().ReadJsonWebToken(token);
    }

    private sealed class StubHttpContextAccessor(HttpContext httpContext) : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; } = httpContext;
    }
}
