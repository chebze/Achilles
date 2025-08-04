using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.OpeningHours;

public class InfoHotelClosedMessage : OutgoingMessage
{
    public InfoHotelClosedMessage(TimeOnly openingTime, bool disconnect) : base(292)
    {
        this.AppendWired(openingTime.Hour);
        this.AppendWired(openingTime.Minute);
        this.AppendBoolean(disconnect);
    }
}
