using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Handshake;

public class SecretKeyMessage : OutgoingMessage
{
    public SecretKeyMessage(string key) : base(1)
    {
        this.AppendString(key);
    }
}
