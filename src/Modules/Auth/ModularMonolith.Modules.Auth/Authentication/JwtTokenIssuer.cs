using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using ModularMonolith.Shared.Infrastructure.Authentication;
using ModularMonolith.Shared.Infrastructure.Multitenancy;

namespace ModularMonolith.Modules.Auth.Authentication;

public sealed class JwtTokenIssuer(IOptions<JwtOptions> options, TimeProvider timeProvider) : IJwtTokenIssuer
{
    private readonly JwtOptions _options = options.Value;

    public string Issue(Guid userId, Guid tenantId, string email)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddMinutes(_options.AccessTokenMinutes),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = userId.ToString(),
                [JwtRegisteredClaimNames.Email] = email,
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
                [TenantClaimTypes.TenantId] = tenantId.ToString()
            }
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
