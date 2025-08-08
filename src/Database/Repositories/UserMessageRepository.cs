using Achilles.Database.Abstractions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories;

public class UserMessageRepository : BaseRepository<UserMessage, int>
{
    public UserMessageRepository(HabboDbContext context) : base(context)
    {
    }
}