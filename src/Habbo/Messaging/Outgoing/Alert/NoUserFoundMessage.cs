using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Alert;

public class NoUserFoundMessage : OutgoingMessage
{
    public NoUserFoundMessage(string username) : base(76)
    {
        this.AppendString(username);
    }
}
