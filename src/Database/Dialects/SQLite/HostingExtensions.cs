using Achilles.Database.Abstractions;

namespace Achilles.Database.Dialects.SQLite;

public static class HostingExtensions
{
    public static IServiceCollection UseSQLite(this IServiceCollection services)
    {
        services.AddDbContext<HabboDbContext, HabboSqliteDbContext>();

        return services;
    }
}