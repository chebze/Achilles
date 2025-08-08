using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Rooms;

public class RoomDoorbellMessage : OutgoingMessage
{
    public RoomDoorbellMessage() : base(91)
    {
    }

    public RoomDoorbellMessage(string username) : base(91)
    {
        this.AppendString(username);
    }
}
