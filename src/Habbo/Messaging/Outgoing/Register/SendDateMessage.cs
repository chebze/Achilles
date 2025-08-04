using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Register;

public class SendDateMessage : OutgoingMessage
{
    public SendDateMessage() : base(163)
    {
        this.Append(DateTime.Now.ToString("dd.MM.yyyy"));
    }
}
