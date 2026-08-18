using DotNetEnv;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Npgsql;

namespace ModularMonolith.Modules.Notifications.Persistence;

public sealed class NotificationsDbContextFactory : IDesignTimeDbContextFactory<NotificationsDbContext>
{
    public NotificationsDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().NoClobber().Load();

        var baseConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings__Postgres is not set");

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            SearchPath = "notifications"
        };

        var optionsBuilder = new DbContextOptionsBuilder<NotificationsDbContext>();
        optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory");
        });

        return new NotificationsDbContext(optionsBuilder.Options);
    }
}
