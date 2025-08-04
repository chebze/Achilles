using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Recycler;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Incoming.User;

[IncomingMessage(191)]
public class GetMessagesMessage : IncomingMessage
{
    public GetMessagesMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }
}