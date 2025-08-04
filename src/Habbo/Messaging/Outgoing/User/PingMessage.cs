using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class PingMessage : OutgoingMessage
{
    public PingMessage() : base(50)
    {
    }
}
