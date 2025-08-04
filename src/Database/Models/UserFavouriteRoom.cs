namespace Achilles.Database.Models;

public class UserFavouriteRoom
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required int RoomId { get; set; }
}