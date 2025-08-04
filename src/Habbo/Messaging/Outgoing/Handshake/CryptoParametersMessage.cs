using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Handshake;

public class CryptoParametersMessage : OutgoingMessage
{
    public CryptoParametersMessage() : base(277)
    {
        this.AppendWired(0);
    }
}

