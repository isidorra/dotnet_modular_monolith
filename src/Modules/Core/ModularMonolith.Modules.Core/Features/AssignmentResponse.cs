using ModularMonolith.Modules.Core.Domain;

namespace ModularMonolith.Modules.Core.Features;

public sealed record AssignmentResponse(
    Guid Id,
    Guid EmployeeId,
    string Title,
    string Description,
    AssignmentStatus Status,
    DateTimeOffset? DueAt,
    DateTimeOffset CreatedAt);
