using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.OpeningHours;

public class InfoHotelClosingMessage : OutgoingMessage
{
    public InfoHotelClosingMessage(DateTime closingTime) : base(291)
    {
        TimeSpan difference = closingTime - DateTime.Now;
        this.AppendWired(difference.Minutes);
    }

    public InfoHotelClosingMessage(TimeSpan closingTime) : base(291)
    {
        this.AppendWired(closingTime.Minutes);
    }
}