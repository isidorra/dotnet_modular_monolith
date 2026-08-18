using Microsoft.EntityFrameworkCore;

using ModularMonolith.Modules.Core.Domain;
using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Core.Persistence;

public sealed class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Assignment> Assignments => Set<Assignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);

        modelBuilder.UseSnakeCaseNames();
    }
}
