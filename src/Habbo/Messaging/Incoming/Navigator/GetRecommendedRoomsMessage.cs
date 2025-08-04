using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(264)]
public class GetRecommendedRoomsMessage : IncomingMessage
{
    public GetRecommendedRoomsMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }
}
