using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class IgnoredListMessage : OutgoingMessage
{
    public IgnoredListMessage(List<string> ignoredUsers) : base(420)
    {
        this.AppendWired(ignoredUsers.Count);
        
        foreach(var username in ignoredUsers)
            this.AppendString(username);
    }
}