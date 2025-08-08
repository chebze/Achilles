using Achilles.Database.Abstractions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories;

public class RoomRepository : BaseRepository<Room, int>
{
    public RoomRepository(HabboDbContext context) : base(context)
    {
    }
}