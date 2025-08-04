using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class SendToHotelViewMessage : OutgoingMessage
{
    public SendToHotelViewMessage() : base(18)
    {
    }
}
