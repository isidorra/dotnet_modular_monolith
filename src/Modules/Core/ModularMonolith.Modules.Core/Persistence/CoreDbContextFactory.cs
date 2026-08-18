using DotNetEnv;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Npgsql;

namespace ModularMonolith.Modules.Core.Persistence;

public sealed class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
    public CoreDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().NoClobber().Load();

        var baseConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings__Postgres is not set");

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            SearchPath = "core"
        };

        var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
        optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory");
        });

        return new CoreDbContext(optionsBuilder.Options);
    }
}
