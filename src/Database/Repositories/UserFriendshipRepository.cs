using Achilles.Database.Abstractions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories;

public class UserFriendshipRepository : BaseRepository<UserFriendship, int>
{
    public UserFriendshipRepository(HabboDbContext context) : base(context)
    {
    }
}   