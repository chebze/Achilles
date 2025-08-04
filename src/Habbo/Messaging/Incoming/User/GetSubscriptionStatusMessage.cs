using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Club;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Incoming.User;

[IncomingMessage(26)]
public class GetSubscriptionStatusMessage : IncomingMessage
{
    public GetSubscriptionStatusMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override OutgoingMessage? Respond(IncomingMessageContext ctx)
    {
        return new ClubInfoMessage(ctx);
    }
}   
