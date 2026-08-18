using System.Linq.Expressions;

using FluentValidation.TestHelper;

using Microsoft.EntityFrameworkCore;

using ModularMonolith.Modules.Core.Features;
using ModularMonolith.Modules.Core.Persistence;

namespace ModularMonolith.Modules.Core.Tests.Features;

public sealed class CreateAssignmentCommandValidatorTests : IDisposable
{
    private readonly CoreDbContext _core = new(
        new DbContextOptionsBuilder<CoreDbContext>()
            .UseNpgsql("Host=localhost;Database=unreachable")
            .Options);

    private readonly CreateAssignmentCommandValidator _validator;

    public CreateAssignmentCommandValidatorTests()
    {
        _validator = new CreateAssignmentCommandValidator(_core);
    }

    [Fact]
    public async Task Rejects_an_empty_employee_id_without_reaching_the_database()
    {
        var result = await ValidateAsync(Command(employeeId: Guid.Empty), x => x.EmployeeId);

        result.ShouldHaveValidationErrorFor(x => x.EmployeeId);
    }

    [Fact]
    public async Task Accepts_a_well_formed_title_and_description()
    {
        var result = await ValidateAsync(Command(), x => x.Title, x => x.Description);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Rejects_an_empty_title()
    {
        var result = await ValidateAsync(Command(title: ""), x => x.Title);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async Task Rejects_a_title_longer_than_200_characters()
    {
        var result = await ValidateAsync(Command(title: new string('a', 201)), x => x.Title);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async Task Accepts_a_title_exactly_at_the_column_length()
    {
        var result = await ValidateAsync(Command(title: new string('a', 200)), x => x.Title);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Rejects_a_description_longer_than_2000_characters()
    {
        var result = await ValidateAsync(Command(description: new string('a', 2001)), x => x.Description);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task Accepts_an_empty_description()
    {
        var result = await ValidateAsync(Command(description: ""), x => x.Description);

        result.ShouldNotHaveAnyValidationErrors();
    }

    public void Dispose()
    {
        _core.Dispose();
    }

    private Task<TestValidationResult<CreateAssignmentCommand>> ValidateAsync(
        CreateAssignmentCommand command,
        params Expression<Func<CreateAssignmentCommand, object>>[] properties)
    {
        return _validator.TestValidateAsync(
            command,
            options => options.IncludeProperties(properties),
            TestContext.Current.CancellationToken);
    }

    private static CreateAssignmentCommand Command(
        Guid? employeeId = null,
        string title = "Ship the release",
        string description = "Cut the tag and publish the notes",
        DateTimeOffset? dueAt = null)
    {
        return new CreateAssignmentCommand(
            employeeId ?? Guid.CreateVersion7(),
            title,
            description,
            dueAt);
    }
}
