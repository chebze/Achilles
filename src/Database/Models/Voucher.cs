namespace Achilles.Database.Models;

public enum VoucherType
{
    Credits = 0,
    Tickets = 1,
    Films = 2,
    Furniture = 3,
    HabboClubMonths = 4
}

public class Voucher : BaseModel<string>
{
    public required string Code { get; set; }
    public required int Value { get; set; }
    public required VoucherType Type { get; set; }
}