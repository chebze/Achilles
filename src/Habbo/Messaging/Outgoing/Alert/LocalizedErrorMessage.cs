using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Alert;

public class LocalizedErrorMessage : OutgoingMessage
{
    public LocalizedErrorMessage(string externalTextEntry) : base(33)
    {
        this.AppendString(externalTextEntry);
    }
}
