using DotNetEnv;
using JasperFx.Resources;
using ModularMonolith.Modules.Auth;
using ModularMonolith.Modules.Core;
using ModularMonolith.Modules.Notifications;
using ModularMonolith.Shared.Infrastructure.Modules;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Postgresql;

Env.TraversePath().NoClobber().Load();

var builder = WebApplication.CreateBuilder(args);

IModule[] modules =
[
    new AuthModule(),
    new CoreModule(),
    new NotificationsModule()
];

foreach (var module in modules)
{
    module.AddModule(builder.Services, builder.Configuration);
}

var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured");

builder.Host.UseWolverine(opts =>
{
    opts.PersistMessagesWithPostgresql(postgresConnectionString, "wolverine");
    opts.UseEntityFrameworkCoreTransactions();
    opts.UseFluentValidation();
    opts.UseRuntimeCompilation();

    foreach (var module in modules)
    {
        opts.Discovery.IncludeAssembly(module.Assembly);
    }
});

builder.Host.UseResourceSetupOnStartup();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    modules = modules.Select(m => m.Name)
}));

app.Run();
