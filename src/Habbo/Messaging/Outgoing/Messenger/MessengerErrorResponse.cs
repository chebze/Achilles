using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Messenger;

public class MessengerErrorResponse : OutgoingMessage
{
    public enum MessengerErrorType
    {
        TargetFriendsListFull = 2,
        TargetDoesNotAccept = 3,
        FriendRequestNotFound = 4,
        FriendRemovalError = 37,
        FriendsListFull = 39,
        ConcurrencyError = 42
    }
    public enum MessengerErrorReason
    {
        FriendsListFullPendingFriend = 1,
        SenderFriendsListFull = 2,
        Concurrency = 42
    }

    public MessengerErrorResponse(
        IncomingMessageContext context,
        MessengerErrorType errorType,
        MessengerErrorReason? reason = null
    ) : base(260)
    {
        this.AppendWired((int) errorType);

        if(reason is not null)
            this.AppendWired((int) reason.Value);
    }
}
