using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Messenger;

public class FriendsRemovedMessage : OutgoingMessage
{
    public FriendsRemovedMessage(List<int> userIds) : base(138)
    {
        this.AppendWired(userIds.Count);
        foreach(var userId in userIds)
            this.AppendWired(userId);
    }
}