using Microsoft.EntityFrameworkCore;

namespace ModularMonolith.Modules.Core.Persistence;

public sealed class CoreDbContext : DbContext
{
    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
    {
    }
}
