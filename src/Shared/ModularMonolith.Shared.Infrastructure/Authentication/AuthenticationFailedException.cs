namespace ModularMonolith.Shared.Infrastructure.Authentication;

public sealed class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException() : base("Invalid credentials")
    {
    }
}
