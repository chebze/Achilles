using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Navigator;

public class NavigateToRoomMessage : OutgoingMessage
{
    public NavigateToRoomMessage(IncomingMessageContext ctx, Room room) : base(59)
    {
        this.AppendString(room.Id.ToString(), (char) 13);
        this.Append(room.Name);
    }
}
