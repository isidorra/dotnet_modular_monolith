using FluentValidation.TestHelper;

using ModularMonolith.Modules.Auth.Features;

namespace ModularMonolith.Modules.Auth.Tests.Features;

public sealed class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Accepts_a_well_formed_command()
    {
        _validator.TestValidate(new LoginCommand("ada@example.com", "S3cret!pass"))
            .ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("@example.com")]
    [InlineData("ada@")]
    public void Rejects_an_email_that_is_missing_or_malformed(string email)
    {
        _validator.TestValidate(new LoginCommand(email, "S3cret!pass"))
            .ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Accepts_a_domain_without_a_top_level_domain()
    {
        _validator.TestValidate(new LoginCommand("ada@localhost", "S3cret!pass"))
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Rejects_an_email_longer_than_256_characters()
    {
        var email = new string('a', 250) + "@example.com";

        _validator.TestValidate(new LoginCommand(email, "S3cret!pass"))
            .ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Rejects_an_empty_password()
    {
        _validator.TestValidate(new LoginCommand("ada@example.com", ""))
            .ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Does_not_apply_the_registration_password_rules_to_login()
    {
        _validator.TestValidate(new LoginCommand("ada@example.com", "weak"))
            .ShouldNotHaveAnyValidationErrors();
    }
}
