using Achilles.Database.Abstractions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories;

public class RoomUserRightRepository : BaseRepository<RoomUserRight, int>
{
    public RoomUserRightRepository(HabboDbContext context) : base(context)
    {
    }
}