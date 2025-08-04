using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Register;

public class ApproveNameResponseMessage : OutgoingMessage
{
    public enum ApproveNameResponseType
    {
        Success = 0,
        TooLong = 1,
        TooShort = 2,
        InvalidCharacters = 3,
        Taken = 4
    }

    public ApproveNameResponseMessage(ApproveNameResponseType type) : base(36)
    {
        this.AppendWired((int) type);
    }
}
