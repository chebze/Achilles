using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Navigator;

public class NoRoomsFoundForUserMessage : OutgoingMessage
{
    public NoRoomsFoundForUserMessage(string username) : base(57)
    {
        this.AppendString(username);
    }
}   

