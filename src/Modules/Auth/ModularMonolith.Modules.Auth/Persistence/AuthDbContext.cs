using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ModularMonolith.Modules.Auth.Domain;
using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Auth.Persistence;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityUserContext<AppUser, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseSnakeCaseNames();
    }
}
