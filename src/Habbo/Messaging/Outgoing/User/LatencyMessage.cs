using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class LatencyMessage : OutgoingMessage
{
    public LatencyMessage(int latency) : base(354)
    {
        this.AppendWired(latency);
    }
}