using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Register;

public class ApprovePasswordResponseMessage : OutgoingMessage
{
    public enum ApprovePasswordResponseType
    {
        Success = 0,
        TooShort = 1,
        TooLong = 2,
        SameAsUsername = 5
    }

    public ApprovePasswordResponseMessage(ApprovePasswordResponseType type) : base(282)
    {
        this.AppendWired((int) type);
    }
}
