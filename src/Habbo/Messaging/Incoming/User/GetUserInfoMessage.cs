using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Incoming.User;

[IncomingMessage(7)]
public class GetUserInfoMessage : IncomingMessage
{
    public GetUserInfoMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override OutgoingMessage? Respond(IncomingMessageContext ctx)
    {
        return new SendUserInfoMessage(ctx);
    }
}
