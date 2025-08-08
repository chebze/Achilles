using Achilles.Database.Abstractions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories;

public class RoomCategoryRepository : BaseRepository<RoomCategory, int>
{
    public RoomCategoryRepository(HabboDbContext context) : base(context)
    {
    }
}