using DotNetEnv;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Auth.Persistence;

public sealed class AuthCatalogDbContextFactory : IDesignTimeDbContextFactory<AuthCatalogDbContext>
{
    public AuthCatalogDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().NoClobber().Load();

        var baseConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings__Postgres is not set");

        var optionsBuilder = new DbContextOptionsBuilder<AuthCatalogDbContext>();

        ModuleDbContextServiceCollectionExtensions.UseSearchPath(
            optionsBuilder,
            baseConnectionString,
            AuthCatalogDbContext.Schema,
            AuthCatalogDbContext.Schema);

        return new AuthCatalogDbContext(optionsBuilder.Options);
    }
}
