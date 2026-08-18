using DotNetEnv;
using JasperFx.Resources;
using ModularMonolith.Modules.Auth;
using ModularMonolith.Modules.Core;
using ModularMonolith.Modules.Notifications;
using ModularMonolith.Shared.Infrastructure.Authentication;
using ModularMonolith.Shared.Infrastructure.Http;
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

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();

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

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "healthy",
        modules = modules.Select(m => m.Name)
    });
}).AllowAnonymous();

var api = app.MapGroup("/api/v1");

foreach (var module in modules)
{
    module.MapEndpoints(api);
}

app.Run();
