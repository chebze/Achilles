using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Messenger;

public class FriendRequestResultMessage : OutgoingMessage
{
    public FriendRequestResultMessage(
        IncomingMessageContext context,
        List<(string username, MessengerErrorResponse.MessengerErrorType errorType)> errors
    ) : base(315)
    {
        this.AppendWired(errors.Count);
        foreach(var (username, errorType) in errors)
        {
            this.AppendString(username);
            this.AppendWired((int)errorType);
        }
    }
}
