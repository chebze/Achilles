namespace Achilles.Database.Models;

public class RoomCategory : BaseModel<int>
{
    public int? ParentId { get; set; }
    
    public required string Name { get; set; }

    public bool IsForPublicSpaces { get; set; }
    public bool IsAssignableToRoom { get; set; }
    public bool AllowTrading { get; set; }

    public int MaxVisitors { get; set; }
}
