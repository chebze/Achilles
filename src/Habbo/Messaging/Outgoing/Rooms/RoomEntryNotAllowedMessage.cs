using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Rooms;

public class RoomEntryNotAllowedMessage : OutgoingMessage
{
    public RoomEntryNotAllowedMessage() : base(131)
    {
    }
}
