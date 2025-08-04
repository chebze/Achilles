using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Register;

public class AgeCheckResponseMessage : OutgoingMessage
{
    public AgeCheckResponseMessage(bool result) : base(164)
    {
        this.AppendWired(result);
    }
}

