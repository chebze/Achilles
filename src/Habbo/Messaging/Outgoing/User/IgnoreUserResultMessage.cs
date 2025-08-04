using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class IgnoreUserResultMessage : OutgoingMessage
{
    public IgnoreUserResultMessage(int result) : base(419)
    {
        this.AppendWired(result);
    }
}