using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Handshake;

public class LoginMessage : OutgoingMessage
{
    public LoginMessage() : base(3)
    {
    }
}