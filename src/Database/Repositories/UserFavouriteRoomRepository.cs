using Achilles.Database.Abstractions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories;

public class UserFavouriteRoomRepository : BaseRepository<UserFavouriteRoom, int>
{
    public UserFavouriteRoomRepository(HabboDbContext context) : base(context)
    {
    }
}