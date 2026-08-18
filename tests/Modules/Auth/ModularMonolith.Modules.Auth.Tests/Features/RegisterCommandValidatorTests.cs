using FluentValidation.TestHelper;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using ModularMonolith.Modules.Auth.Authentication;
using ModularMonolith.Modules.Auth.Features;
using ModularMonolith.Modules.Auth.Persistence;

namespace ModularMonolith.Modules.Auth.Tests.Features;

public sealed class RegisterCommandValidatorTests : IDisposable
{
    private readonly AuthCatalogDbContext _catalog = new(
        new DbContextOptionsBuilder<AuthCatalogDbContext>()
            .UseNpgsql("Host=localhost;Database=unreachable")
            .Options);

    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator(_catalog, new UpperInvariantLookupNormalizer());
    }

    [Fact]
    public async Task Accepts_a_password_that_meets_every_requirement()
    {
        var result = await ValidatePasswordAsync("S3cret!pass");

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("S3cr!t")]
    [InlineData("s3cret!pass")]
    [InlineData("S3CRET!PASS")]
    [InlineData("Secret!pass")]
    [InlineData("S3cretpass")]
    public async Task Rejects_a_password_that_misses_a_requirement(string password)
    {
        var result = await ValidatePasswordAsync(password);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Rejects_a_password_one_character_below_the_identity_policy_length()
    {
        var result = await ValidatePasswordAsync(PasswordOfLength(PasswordPolicy.RequiredLength - 1));

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Accepts_a_password_exactly_at_the_identity_policy_length()
    {
        var result = await ValidatePasswordAsync(PasswordOfLength(PasswordPolicy.RequiredLength));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Rejects_an_empty_tenant_name()
    {
        var result = await ValidateAsync(Command(tenantName: ""), x => x.TenantName);

        result.ShouldHaveValidationErrorFor(x => x.TenantName);
    }

    [Fact]
    public async Task Rejects_a_tenant_name_longer_than_200_characters()
    {
        var result = await ValidateAsync(Command(tenantName: new string('a', 201)), x => x.TenantName);

        result.ShouldHaveValidationErrorFor(x => x.TenantName);
    }

    [Fact]
    public async Task Rejects_an_empty_first_name()
    {
        var result = await ValidateAsync(Command(firstName: ""), x => x.FirstName);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task Rejects_a_last_name_longer_than_100_characters()
    {
        var result = await ValidateAsync(Command(lastName: new string('a', 101)), x => x.LastName);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    public void Dispose()
    {
        _catalog.Dispose();
    }

    private Task<TestValidationResult<RegisterCommand>> ValidatePasswordAsync(string password)
    {
        return ValidateAsync(Command(password: password), x => x.Password);
    }

    private Task<TestValidationResult<RegisterCommand>> ValidateAsync(
        RegisterCommand command,
        params System.Linq.Expressions.Expression<Func<RegisterCommand, object>>[] properties)
    {
        return _validator.TestValidateAsync(
            command,
            options => options.IncludeProperties(properties),
            TestContext.Current.CancellationToken);
    }

    private static RegisterCommand Command(
        string tenantName = "Acme",
        string email = "ada@example.com",
        string password = "S3cret!pass",
        string firstName = "Ada",
        string lastName = "Lovelace")
    {
        return new RegisterCommand(tenantName, email, password, firstName, lastName);
    }

    private static string PasswordOfLength(int length)
    {
        return "Aa1!".PadRight(length, 'x');
    }
}