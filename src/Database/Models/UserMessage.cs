namespace Achilles.Database.Models;

public class UserMessage
{
    public int Id { get; set; }

    public required int FromUserId { get; set; }
    public required int ToUserId { get; set; }
    
    public required string Message { get; set; }

    public DateTime SentAt { get; set; }
}
