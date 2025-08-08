using Achilles.Database.Abstractions;

namespace Achilles.Database.Repositories.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<RoomRepository>();
        services.AddScoped<RoomCategoryRepository>();
        services.AddScoped<RoomModelRepository>();
        services.AddScoped<RoomUserRightRepository>();

        services.AddScoped<UserRepository>();
        services.AddScoped<UserFavouriteRoomRepository>();
        services.AddScoped<UserFriendshipRepository>();
        services.AddScoped<UserMessageRepository>();
        services.AddScoped<UserTransactionRepository>();
        
        services.AddScoped<VoucherRepository>();

        return services;
    }
}