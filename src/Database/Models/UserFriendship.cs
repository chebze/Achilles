namespace Achilles.Database.Models;

public class UserFriendship
{
    public int Id { get; set; }

    public required int FromUserId { get; set; }
    public required int ToUserId { get; set; }

    public required bool IsAccepted { get; set; }
    public bool IsDeclined { get; set; } = false;


    public required DateTime SentAt { get; set; }
    public DateTime? AcceptedAt { get; set; } = null;
    public DateTime? DeclinedAt { get; set; } = null;
}
