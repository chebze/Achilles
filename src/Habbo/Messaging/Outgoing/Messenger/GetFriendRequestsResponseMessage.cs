using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.Messenger;

public class GetFriendRequestsResponseMessage : OutgoingMessage
{
    public GetFriendRequestsResponseMessage(int userId, string username) : base(132)
    {
        this.AppendWired(userId);
        this.AppendString(username);
    }
}

