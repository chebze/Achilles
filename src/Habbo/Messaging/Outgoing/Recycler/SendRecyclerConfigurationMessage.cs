using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Recycler;

public class SendRecyclerConfigurationMessage : OutgoingMessage
{
    public SendRecyclerConfigurationMessage() : base(303)
    {
        this
            .AppendBoolean(false)
            .AppendWired(0)
            .AppendWired(0)
            .AppendWired(0)
            .AppendWired(0);
    }
}