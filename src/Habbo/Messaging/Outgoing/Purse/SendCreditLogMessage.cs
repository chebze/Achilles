using System.Numerics;
using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.Purse;

public class SendCreditLogMessage : OutgoingMessage
{
    public static class TransactionSystemNames
    {
        public const string HabboClub = "club_habbo";
        public const string Refunds = "refund";
        public const string CataloguePurchase = "catalogue_purchase";
        public const string Voucher = "voucher";
    }

    public class CreditLogEntry
    {
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public required decimal CreditValue { get; set; }
        public required decimal RealValue { get; set; }
        public required string Currency { get; set; }
        public required string TransactionSystemName { get; set; }

        public static implicit operator CreditLogEntry(UserTransaction transaction)
        {
            return new CreditLogEntry
            {
                Date = transaction.Date,
                Time = transaction.Time,
                CreditValue = transaction.CreditValue,
                RealValue = transaction.RealValue,
                Currency = transaction.Currency,
                TransactionSystemName = transaction.TransactionSystemName
            };
        }
    }

    public SendCreditLogMessage(IncomingMessageContext ctx) : base(209)
    {
        var user = ctx.User;
        if (user is null)
        {
            this.Build([]);
            return;
        }

        List<UserTransaction> transactions = ctx.Database.UserTransactions.Where(t => t.UserId == user.Id).ToList();

        this.Build(transactions.Select(transaction => (CreditLogEntry)transaction).ToList());
    }
    public SendCreditLogMessage(List<CreditLogEntry> entries) : base(209)
    {
        this.Build(entries);
    }

    private void Build(List<CreditLogEntry> entries)
    {
        foreach (var entry in entries)
        {
            this.AppendString(entry.Date.ToString("yyyy-MM-dd"), Convert.ToChar(9));
            this.AppendString(entry.Time.ToString("HH:mm:ss"), Convert.ToChar(9));
            this.AppendString(entry.CreditValue.ToString(), Convert.ToChar(9));
            this.AppendString(entry.RealValue.ToString(), Convert.ToChar(9));
            this.AppendString(entry.Currency, Convert.ToChar(9));
            this.AppendString(entry.TransactionSystemName, Convert.ToChar(9));
            this.AppendChar(Convert.ToChar(13));
        }
    }
}