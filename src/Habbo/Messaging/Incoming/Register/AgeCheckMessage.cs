using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Register;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Incoming.Register;

[IncomingMessage(46)]
public class AgeCheckMessage : IncomingMessage
{
    public AgeCheckMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override OutgoingMessage? Respond(IncomingMessageContext ctx)
    {
        return new AgeCheckResponseMessage(true);
    }
}