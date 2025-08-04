using Achilles.Habbo.Configuration;
using Achilles.Habbo.Handlers;

namespace Achilles.Habbo.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection UseHabboServer(this IServiceCollection services)
    {
        services.AddSingleton<HabboConfiguration>(s => s.GetRequiredService<IConfiguration>().GetSection("Habbo").Get<HabboConfiguration>() ?? throw new Exception("Habbo configuration missing"));
        services.AddSingleton<ConnectionHandler>();

        return services;
    }

    public static async Task<IHost> StartHabboServer(this IHost host)
    {
        host.Services.GetRequiredService<ConnectionHandler>();
        
        await Task.Delay(0);
        return host;
    }
}