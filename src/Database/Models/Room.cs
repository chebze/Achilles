namespace Achilles.Database.Models;

public enum RoomAccessType
{
    Open = 0,
    Closed = 1,
    Password = 2
}

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int MaxVisitors { get; set; }

    public required string RoomModelId { get; set; }
    public required int RoomCategoryId { get; set; }
    public required int OwnerId { get; set; }

    public required bool ShowOwnerName { get; set; }
    public required bool AllSuperUsers { get; set; }

    public required RoomAccessType AccessType { get; set; }
    public string? Password { get; set; } = null;

    public string? CCTs { get; set; } = null;
    public int Floor { get; set; }
    public int Wallpaper { get; set; }
}
