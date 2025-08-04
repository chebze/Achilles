using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class UpdateAccountResponseMessage : OutgoingMessage
{
    public enum UpdateAccountResponseType
    {
        Success = 0,
        IncorrectPassword = 1,
        IncorrectBirthday = 2
    }

    public UpdateAccountResponseMessage(UpdateAccountResponseType type) : base(169)
    {
        this.AppendWired((int) type);
    }
}