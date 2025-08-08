namespace Achilles.Database.Models;

public class RoomUserRight : BaseModel<int>
{
    public required int RoomId { get; set; }
    public required int UserId { get; set; }
}
