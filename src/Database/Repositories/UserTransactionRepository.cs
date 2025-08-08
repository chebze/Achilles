using Achilles.Database.Abstractions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories;

public class UserTransactionRepository : BaseRepository<UserTransaction, int>
{
    public UserTransactionRepository(HabboDbContext context) : base(context)
    {
    }
}