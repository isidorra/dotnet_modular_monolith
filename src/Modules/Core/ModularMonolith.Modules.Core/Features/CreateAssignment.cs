using FluentValidation;

using Microsoft.EntityFrameworkCore;

using ModularMonolith.Modules.Core.Domain;
using ModularMonolith.Modules.Core.Persistence;

namespace ModularMonolith.Modules.Core.Features;

public sealed record CreateAssignmentCommand(
    Guid EmployeeId,
    string Title,
    string Description,
    DateTimeOffset? DueAt);

public sealed class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator(CoreDbContext core)
    {
        RuleFor(x => x.EmployeeId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (employeeId, cancellationToken) =>
            {
                return await core.Employees
                    .AnyAsync(employee => employee.Id == employeeId, cancellationToken);
            })
            .WithMessage("No employee exists with this identifier");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}

public static class CreateAssignmentHandler
{
    public static async Task<AssignmentResponse> Handle(
        CreateAssignmentCommand command,
        CoreDbContext core,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var assignment = new Assignment
        {
            Id = Guid.CreateVersion7(),
            EmployeeId = command.EmployeeId,
            Title = command.Title,
            Description = command.Description,
            Status = AssignmentStatus.Todo,
            DueAt = command.DueAt,
            CreatedAt = timeProvider.GetUtcNow()
        };

        core.Assignments.Add(assignment);

        await core.SaveChangesAsync(cancellationToken);

        return new AssignmentResponse(
            assignment.Id,
            assignment.EmployeeId,
            assignment.Title,
            assignment.Description,
            assignment.Status,
            assignment.DueAt,
            assignment.CreatedAt);
    }
}