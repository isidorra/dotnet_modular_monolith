using Microsoft.EntityFrameworkCore;

namespace ModularMonolith.Modules.Auth.Persistence;

public sealed class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }
}
