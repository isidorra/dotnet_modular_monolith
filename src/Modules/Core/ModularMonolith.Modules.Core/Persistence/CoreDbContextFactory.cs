using DotNetEnv;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Core.Persistence;

public sealed class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
    public CoreDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().NoClobber().Load();

        var baseConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings__Postgres is not set");

        var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();

        ModuleDbContextServiceCollectionExtensions.UseSearchPath(
            optionsBuilder,
            baseConnectionString,
            CoreModule.SchemaPrefix,
            null);

        return new CoreDbContext(optionsBuilder.Options);
    }
}