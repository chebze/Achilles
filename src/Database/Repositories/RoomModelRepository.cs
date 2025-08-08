using Achilles.Database.Abstractions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories;

public class RoomModelRepository : BaseRepository<RoomModel, string>
{
    public RoomModelRepository(HabboDbContext context) : base(context)
    {
    }
}