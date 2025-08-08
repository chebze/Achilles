using Achilles.Shared.Extensions;
using Achilles.TCP.Extensions;
using Achilles.Habbo.Extensions;
using Achilles.Database.Dialects.SQLite;
using Achilles.Database.Repositories.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .UseSQLite()
    .AddRepositories();

builder.Services
    .UseSharedServices()
    .UseTcpServer()
    .UseHabboServer();

var app = builder.Build();
await app.StartHabboServer();
await app.RunAsync();