using Achilles.Database.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Database.Dialects.SQLite;

public class HabboSqliteDbContext : HabboDbContext
{
    public HabboSqliteDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
}