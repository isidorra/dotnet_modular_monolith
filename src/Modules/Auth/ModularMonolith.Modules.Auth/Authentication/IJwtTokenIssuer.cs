namespace ModularMonolith.Modules.Auth.Authentication;

public interface IJwtTokenIssuer
{
    string Issue(Guid userId, Guid tenantId, string email);
}
