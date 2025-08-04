using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Register;

public class ApproveEmailResponseMessage : OutgoingMessage
{
    public ApproveEmailResponseMessage() : base(271)
    {
    }
}
