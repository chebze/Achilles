using Achilles.Database.Abstractions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories;

public class UserRepository : BaseRepository<User, int>
{
    public UserRepository(HabboDbContext context) : base(context)
    {
    }
}