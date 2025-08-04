using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.Club;

public class SubscriptionInfoMessage : OutgoingMessage
{
    private int GetRemainingSubscriptionDaysInCurrentMonth(DateTime startDate, DateTime expirationDate)
    {
        DateTime lastDayOfCurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
        DateTime lastDayOfExpirationMonth = new DateTime(expirationDate.Year, expirationDate.Month, 1).AddMonths(1).AddDays(-1);

        if(expirationDate < lastDayOfCurrentMonth)
        {
            return (expirationDate - DateTime.Now).Days;
        }else{
            // return days remaining in the current month
            return (lastDayOfCurrentMonth - DateTime.Now).Days;
        }
    }
    private int GetMonthsSinceSubscriptionStart(DateTime startDate)
    {
        return (DateTime.Now.Year - startDate.Year) * 12 + DateTime.Now.Month - startDate.Month;
    }
    private int GetSubscriptionMonthsRemaining(DateTime startDate, DateTime expirationDate)
    {
        return (expirationDate.Year - DateTime.Now.Year) * 12 + expirationDate.Month - DateTime.Now.Month;
    }

    public SubscriptionInfoMessage(string subscriptionType, DateTime? startDate, DateTime? expirationDate, bool openSubscriptionDialog = false) : base(7)
    {
        this.AppendString(subscriptionType);

        if(!startDate.HasValue || !expirationDate.HasValue)
        {
            return;
        }
        if(startDate.Value > DateTime.Now)
        {
            return;
        }
        if(expirationDate.Value < DateTime.Now)
        {
            return;
        }

        int remainingSubscriptionDaysInThisMonth = GetRemainingSubscriptionDaysInCurrentMonth(startDate.Value, expirationDate.Value);
        int monthsSinceSubscriptionStart = GetMonthsSinceSubscriptionStart(startDate.Value);
        int subscriptionMonthsRemaining = GetSubscriptionMonthsRemaining(startDate.Value, expirationDate.Value);

        this.AppendWired(remainingSubscriptionDaysInThisMonth);
        this.AppendWired(monthsSinceSubscriptionStart);
        this.AppendWired(subscriptionMonthsRemaining);

        this.AppendWired(openSubscriptionDialog ? 2 : 1);
    }
}

public class ClubInfoMessage : SubscriptionInfoMessage
{
    public ClubInfoMessage(DateTime? startDate, DateTime? expirationDate, bool openSubscriptionDialog = false)
        : base("club_habbo", startDate, expirationDate, openSubscriptionDialog)
    {
    }

    public ClubInfoMessage(IncomingMessageContext ctx)
        : base("club_habbo", ctx.User?.ClubSubscriptionStart, ctx.User?.ClubSubscriptionEnd, false)
    {
        
    }
}