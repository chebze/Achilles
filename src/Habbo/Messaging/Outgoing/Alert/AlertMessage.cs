using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Alert;

public class AlertMessage : OutgoingMessage
{
    public AlertMessage(string message) : base(139)
    {
        this.AppendString(message);
    }
}
