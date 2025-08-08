using Achilles.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Database.Abstractions;

public abstract class HabboDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserFriendship> UserFriendships { get; set; }
    public DbSet<UserMessage> UserMessages { get; set; }
    public DbSet<UserTransaction> UserTransactions { get; set; }
    public DbSet<UserFavouriteRoom> UserFavouriteRooms { get; set; }

    public DbSet<RoomCategory> RoomCategories { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomModel> RoomModels { get; set; }
    public DbSet<RoomUserRight> RoomUserRights { get; set; }

    public DbSet<Voucher> Vouchers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.SSOTicket).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Ignore(e => e.IsClubMember);
        });

        modelBuilder.Entity<UserFriendship>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.FromUserId);
            entity.HasIndex(e => e.ToUserId);
            entity.HasIndex(e => new { e.FromUserId, e.ToUserId }).IsUnique();
        });

        modelBuilder.Entity<UserMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.FromUserId);
            entity.HasIndex(e => e.ToUserId);
        });

        modelBuilder.Entity<RoomCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Name).IsUnique();
        });
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.RoomCategoryId);
            entity.HasIndex(e => e.OwnerId);
        });
        modelBuilder.Entity<UserTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Time);
            entity.HasIndex(e => e.TransactionSystemName);
        });
        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Code).IsUnique();
        });
        modelBuilder.Entity<RoomModel>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        base.OnModelCreating(modelBuilder);
    }
}
