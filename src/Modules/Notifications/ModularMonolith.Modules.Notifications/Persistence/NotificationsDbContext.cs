using Microsoft.EntityFrameworkCore;

namespace ModularMonolith.Modules.Notifications.Persistence;

public sealed class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options)
    {
    }
}
