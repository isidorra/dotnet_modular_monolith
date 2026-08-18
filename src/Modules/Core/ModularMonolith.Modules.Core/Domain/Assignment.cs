namespace ModularMonolith.Modules.Core.Domain;

public sealed class Assignment
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public AssignmentStatus Status { get; set; }

    public DateTimeOffset? DueAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Employee Employee { get; set; }
}