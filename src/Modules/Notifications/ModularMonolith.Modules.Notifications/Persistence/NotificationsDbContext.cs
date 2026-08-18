using Microsoft.EntityFrameworkCore;

namespace ModularMonolith.Modules.Notifications.Persistence;

public sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options);
