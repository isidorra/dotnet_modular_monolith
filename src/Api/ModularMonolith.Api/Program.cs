using DotNetEnv;
using ModularMonolith.Modules.Auth;
using ModularMonolith.Modules.Core;
using ModularMonolith.Modules.Notifications;
using ModularMonolith.Shared.Infrastructure.Modules;

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

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    modules = modules.Select(m => m.Name)
}));

app.Run();
