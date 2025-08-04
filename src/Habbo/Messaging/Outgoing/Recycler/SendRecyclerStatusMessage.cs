using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Recycler;

public class SendRecyclerStatusMessage : OutgoingMessage
{
    public SendRecyclerStatusMessage() : base(304)
    {
        this
            .AppendWired(0);
    }
}