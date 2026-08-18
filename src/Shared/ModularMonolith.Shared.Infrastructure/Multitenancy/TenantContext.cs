using Microsoft.AspNetCore.Http;

namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public sealed class TenantContext(TenantScopeState scopeState, IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    public Guid TenantId
    {
        get
        {
            if (scopeState.TenantId is { } scopedTenantId)
            {
                return scopedTenantId;
            }

            var claim = httpContextAccessor.HttpContext?.User?.FindFirst(TenantClaimTypes.TenantId)?.Value;

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
