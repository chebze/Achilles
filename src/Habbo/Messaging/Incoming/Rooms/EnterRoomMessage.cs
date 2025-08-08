using Achilles.Database.Models;
using Achilles.Habbo.Data;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Rooms;

[IncomingMessage(59)]
public class EnterRoomMessage : IncomingMessage
{
    public int RoomId { get; set; }

    public EnterRoomMessage(MessageHeader header, string content) : base(header, content)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        Room? room = await ctx.Database.Rooms.FirstOrDefaultAsync(r => r.Id == this.RoomId);
        if(room is null)
            return;

        RoomModel? roomModel = await ctx.Database.RoomModels.FirstOrDefaultAsync(rm => rm.Id == room.RoomModelId);
        if(roomModel is null)
            return;

        if(ctx.Room is not null)
            ctx.Connection.Metadata.Remove(ctx.Room);
        if(ctx.UserRoomState is not null)
            ctx.Connection.Metadata.Remove(ctx.UserRoomState);

        ctx.Connection.Metadata.Add(room);
        ctx.Connection.Metadata.Add(new UserRoomState
        {
            Position = roomModel.GetDoorPosition()
        });
    }
}
