namespace Achilles.Database.Models;

public class RoomCategory
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    
    public required string Name { get; set; }

    public bool IsForPublicSpaces { get; set; }
    public bool IsAssignableToRoom { get; set; }
    public bool AllowTrading { get; set; }

    public int MaxVisitors { get; set; }
}
