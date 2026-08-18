using DotNetEnv;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Notifications.Persistence;

public sealed class NotificationsDbContextFactory : IDesignTimeDbContextFactory<NotificationsDbContext>
{
    public NotificationsDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().NoClobber().Load();

        var baseConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings__Postgres is not set");

        var optionsBuilder = new DbContextOptionsBuilder<NotificationsDbContext>();

        ModuleDbContextServiceCollectionExtensions.UseSearchPath(
            optionsBuilder,
            baseConnectionString,
            NotificationsModule.SchemaPrefix,
            null);

        return new NotificationsDbContext(optionsBuilder.Options);
    }
}