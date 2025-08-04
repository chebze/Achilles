using Achilles.Shared.Extensions;
using Achilles.TCP.Extensions;
using Achilles.Habbo.Extensions;
using Achilles.Database.Dialects.SQLite;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .UseSQLite()
    .UseSharedServices()
    .UseTcpServer()
    .UseHabboServer();

var app = builder.Build();
await app.StartHabboServer();
await app.RunAsync();