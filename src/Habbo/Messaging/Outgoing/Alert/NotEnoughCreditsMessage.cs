using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Alert;

public class NotEnoughCreditsMessage : OutgoingMessage
{
    public NotEnoughCreditsMessage() : base(68)
    {
    }
}

