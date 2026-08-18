using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using ModularMonolith.Shared.Infrastructure.Multitenancy;

namespace ModularMonolith.Shared.Tests.Multitenancy;

public sealed class TenantContextTests
{
    private static readonly Guid ScopedTenantId = Guid.Parse("01a01672-43e5-7de8-98fb-3597d4e38de1");
    private static readonly Guid ClaimedTenantId = Guid.Parse("01a01672-9c2f-7f11-8a3d-6b4e0d5c1a92");

    [Fact]
    public void An_explicit_scope_wins_over_the_claim()
    {
        var context = Create(ScopedTenantId, new Claim(TenantClaimTypes.TenantId, ClaimedTenantId.ToString()));

        context.TenantId.ShouldBe(ScopedTenantId);
    }

    [Fact]
    public void Falls_back_to_the_tenant_claim_when_no_scope_is_bound()
    {
        var context = Create(null, new Claim(TenantClaimTypes.TenantId, ClaimedTenantId.ToString()));

        context.TenantId.ShouldBe(ClaimedTenantId);
    }

    [Fact]
    public void Throws_when_the_principal_carries_no_tenant_claim()
    {
        var context = Create(null);

        Should.Throw<InvalidOperationException>(() => context.TenantId)
            .Message.ShouldContain(TenantClaimTypes.TenantId);
    }

    [Fact]
    public void Throws_when_there_is_no_scope_and_no_http_context()
    {
        var context = new TenantContext(new TenantScopeState(), new StubHttpContextAccessor(null));

        Should.Throw<InvalidOperationException>(() => context.TenantId);
    }

    [Fact]
    public void Throws_when_the_tenant_claim_is_not_a_guid()
    {
        var context = Create(null, new Claim(TenantClaimTypes.TenantId, "not-a-guid"));

        Should.Throw<InvalidOperationException>(() => context.TenantId)
            .Message.ShouldContain("not-a-guid");
    }

    private static TenantContext Create(Guid? scopedTenantId, params Claim[] claims)
    {
        var scopeState = new TenantScopeState();

        if (scopedTenantId is { } tenantId)
        {
            scopeState.Bind(tenantId, null, null);
        }

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims))
        };

        return new TenantContext(scopeState, new StubHttpContextAccessor(httpContext));
    }

    private sealed class StubHttpContextAccessor(HttpContext httpContext) : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; } = httpContext;
    }
}
