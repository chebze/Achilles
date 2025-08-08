using Achilles.Database.Abstractions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories;

public class VoucherRepository : BaseRepository<Voucher, string>
{
    public VoucherRepository(HabboDbContext context) : base(context)
    {
    }
}