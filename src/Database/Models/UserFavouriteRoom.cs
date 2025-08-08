namespace Achilles.Database.Models;

public class UserFavouriteRoom : BaseModel<int>
{
    public required int UserId { get; set; }
    public required int RoomId { get; set; }
}