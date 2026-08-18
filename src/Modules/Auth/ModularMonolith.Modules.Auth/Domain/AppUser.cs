using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Modules.Auth.Domain;

public sealed class AppUser : IdentityUser<Guid>
{
    public DateTimeOffset CreatedAt { get; set; }
}
