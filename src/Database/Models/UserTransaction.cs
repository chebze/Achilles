namespace Achilles.Database.Models;

public class UserTransaction : BaseModel<int>
{
    public required int UserId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public required decimal CreditValue { get; set; }
    public required decimal RealValue { get; set; }
    public required string Currency { get; set; }
    public required string TransactionSystemName { get; set; }
}
