using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Rooms;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Rooms;

[IncomingMessage(60)]
public class GetHeightmapMessage : IncomingMessage
{
    public GetHeightmapMessage(MessageHeader header, string content) : base(header, content)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if(ctx.Room is null)
            return null;

        RoomModel? roomModel = await ctx.Database.RoomModels.FirstOrDefaultAsync(rm => rm.Id == ctx.Room.RoomModelId);
        if(roomModel is null)
            return null;
        
        if(roomModel.Heightmap is null or { Length: < 1 })
            return null;

        return new GetHeightmapResponseMessage(roomModel);
    }
}
