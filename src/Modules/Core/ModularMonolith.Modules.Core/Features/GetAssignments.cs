using FluentValidation;

using Microsoft.EntityFrameworkCore;

using ModularMonolith.Modules.Core.Persistence;
using ModularMonolith.SharedKernel.Pagination;

namespace ModularMonolith.Modules.Core.Features;

public sealed record GetAssignmentsQuery(int Page, int PageSize);

public sealed class GetAssignmentsQueryValidator : AbstractValidator<GetAssignmentsQuery>
{
    public GetAssignmentsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PageDefaults.MaxSize);
    }
}

public static class GetAssignmentsHandler
{
    public static async Task<PagedResult<AssignmentResponse>> Handle(
        GetAssignmentsQuery query,
        CoreDbContext core,
        CancellationToken cancellationToken)
    {
        var assignments = core.Assignments.AsNoTracking();
        var totalCount = await assignments.CountAsync(cancellationToken);

        var items = await assignments
            .OrderByDescending(assignment => assignment.CreatedAt)
            .ThenByDescending(assignment => assignment.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(assignment => new AssignmentResponse(
                assignment.Id,
                assignment.EmployeeId,
                assignment.Title,
                assignment.Description,
                assignment.Status,
                assignment.DueAt,
                assignment.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<AssignmentResponse>(items, query.Page, query.PageSize, totalCount);
    }
}