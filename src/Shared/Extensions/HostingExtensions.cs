using Achilles.Shared.Abstractions;
using Achilles.Shared.Managers;

namespace Achilles.Shared.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection UseSharedServices(this IServiceCollection services)
    {
        services.AddSingleton<IEventManager, EventManager>();
        return services;
    }
}