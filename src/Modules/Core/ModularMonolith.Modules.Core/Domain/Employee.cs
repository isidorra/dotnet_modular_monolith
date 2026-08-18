namespace ModularMonolith.Modules.Core.Domain;

public sealed class Employee
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<Assignment> Assignments { get; set; } = [];
}
