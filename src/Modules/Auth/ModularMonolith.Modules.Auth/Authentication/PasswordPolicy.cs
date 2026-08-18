using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Modules.Auth.Authentication;

public static class PasswordPolicy
{
    public const int RequiredLength = 8;

    public static void Apply(PasswordOptions options)
    {
        options.RequiredLength = RequiredLength;
        options.RequireDigit = true;
        options.RequireLowercase = true;
        options.RequireUppercase = true;
        options.RequireNonAlphanumeric = true;
        options.RequiredUniqueChars = 1;
    }
}
