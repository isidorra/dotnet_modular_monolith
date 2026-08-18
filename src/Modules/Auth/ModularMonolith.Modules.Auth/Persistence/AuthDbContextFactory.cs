using DotNetEnv;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Npgsql;

namespace ModularMonolith.Modules.Auth.Persistence;

public sealed class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().NoClobber().Load();

        var baseConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings__Postgres is not set");

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            SearchPath = "auth"
        };

        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory");
        });

        return new AuthDbContext(optionsBuilder.Options);
    }
}
