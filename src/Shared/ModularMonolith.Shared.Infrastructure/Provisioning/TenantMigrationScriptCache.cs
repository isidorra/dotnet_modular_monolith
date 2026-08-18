using System.Collections.Concurrent;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

using ModularMonolith.Shared.Infrastructure.Multitenancy;

namespace ModularMonolith.Shared.Infrastructure.Provisioning;

public sealed class TenantMigrationScriptCache
{
    private const MigrationsSqlGenerationOptions ScriptOptions =
        MigrationsSqlGenerationOptions.Idempotent | MigrationsSqlGenerationOptions.NoTransactions;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<Type, string> _scripts = new();

    public TenantMigrationScriptCache(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public string ScriptFor(TenantSchemaDescriptor descriptor)
    {
        return _scripts.GetOrAdd(descriptor.ContextType, Generate);
    }

    private string Generate(Type contextType)
    {
        using var scope = _scopeFactory.CreateScope();

        scope.ServiceProvider.GetRequiredService<TenantScopeState>().Bind(Guid.Empty, null, null);

        var context = (DbContext)scope.ServiceProvider.GetRequiredService(contextType);

        return context.GetService<IMigrator>().GenerateScript(null, null, ScriptOptions);
    }
}
