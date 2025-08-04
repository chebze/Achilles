using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Handshake;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Incoming.Handshake;

[IncomingMessage(206)]
public class InitCryptoMessage : IncomingMessage
{
    public InitCryptoMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }   

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override OutgoingMessage? Respond(IncomingMessageContext ctx)
    {
        return new CryptoParametersMessage();
    }
}
