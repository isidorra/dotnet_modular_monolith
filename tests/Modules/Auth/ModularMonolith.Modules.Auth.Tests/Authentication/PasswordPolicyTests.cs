using Microsoft.AspNetCore.Identity;

using ModularMonolith.Modules.Auth.Authentication;

namespace ModularMonolith.Modules.Auth.Tests.Authentication;

public sealed class PasswordPolicyTests
{
    [Fact]
    public void Apply_configures_every_identity_requirement_the_validator_mirrors()
    {
        var options = new PasswordOptions();

        PasswordPolicy.Apply(options);

        options.RequiredLength.ShouldBe(PasswordPolicy.RequiredLength);
        options.RequireDigit.ShouldBeTrue();
        options.RequireLowercase.ShouldBeTrue();
        options.RequireUppercase.ShouldBeTrue();
        options.RequireNonAlphanumeric.ShouldBeTrue();
        options.RequiredUniqueChars.ShouldBe(1);
    }
}