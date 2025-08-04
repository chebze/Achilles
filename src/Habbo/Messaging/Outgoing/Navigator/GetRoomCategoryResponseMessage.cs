using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Navigator;

public class GetRoomCategoryResponseMessage : OutgoingMessage
{
    public GetRoomCategoryResponseMessage(IncomingMessageContext ctx, Room room) : base(222)
    {
        this.AppendWired(room.Id);
        this.AppendWired(room.RoomCategoryId);
    }
}