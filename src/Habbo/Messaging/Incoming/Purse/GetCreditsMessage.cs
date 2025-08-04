using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Purse;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Incoming.Purse;

[IncomingMessage(8)]
public class GetCreditsMessage : IncomingMessage
{
    public GetCreditsMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }
    
    public override OutgoingMessage? Respond(IncomingMessageContext ctx)
    {
        return new SendCreditsMessage(ctx);
    }
}