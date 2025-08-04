using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Achilles.TCP.Abstractions;
using Achilles.TCP.Configuration;
using Achilles.TCP.Handlers;
using Achilles.TCP.Managers;
using Achilles.TCP.Services;

namespace Achilles.TCP.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection UseTcpServer(this IServiceCollection services)
    {
        services.AddSingleton<TcpServerConfiguration>(s => s.GetRequiredService<IConfiguration>().GetSection("Server").Get<TcpServerConfiguration>() ?? throw new Exception("Server configuration missing"));

        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddSingleton<IConnectionHandler, ConnectionHandler>();
        services.AddHostedService<ConnectionPingingService>();

        services.AddSingleton<ITcpServerService, TcpServerService>();
        services.AddSingleton<ITcpServerHostedService, TcpServerHostedService>();
        services.AddHostedService<TcpServerHostedService>();

        services.AddSingleton<IMessageHandler, MessageHandler>();

        return services;
    }
}