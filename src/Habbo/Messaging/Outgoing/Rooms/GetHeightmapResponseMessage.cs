using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Rooms;

public class GetHeightmapResponseMessage : OutgoingMessage
{
    public GetHeightmapResponseMessage(RoomModel roomModel) : base(31)
    {
        this.Append(roomModel.Heightmap);
    }
}
