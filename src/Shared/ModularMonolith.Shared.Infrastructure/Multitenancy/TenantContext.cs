using Microsoft.AspNetCore.Http;

namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public sealed class TenantContext : ITenantContext
{
    private readonly TenantScopeState _scopeState;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(TenantScopeState scopeState, IHttpContextAccessor httpContextAccessor)
    {
        _scopeState = scopeState;
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId
    {
        get
        {
            if (_scopeState.TenantId is { } scopedTenantId)
            {
                return scopedTenantId;
            }

            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(TenantClaimTypes.TenantId)?.Value;

            if (claim is null)
            {
                throw new InvalidOperationException(
                    $"No tenant in scope: no explicit tenant scope and no '{TenantClaimTypes.TenantId}' claim on the current principal");
            }

            if (!Guid.TryParse(claim, out var tenantId))
            {
                throw new InvalidOperationException(
                    $"The '{TenantClaimTypes.TenantId}' claim '{claim}' is not a valid GUID");
            }

            return tenantId;
        }
    }
}
