using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.Habbo.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(23)]
public class DeleteRoomMessage : IncomingMessage
{
    public int RoomId { get; set; }

    public DeleteRoomMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.RoomId = content.ReadWiredInt();
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return;

        Room? room = await ctx.Database.Rooms.FirstOrDefaultAsync(x => x.Id == this.RoomId);
        if(room is null)
            return;

        if(room.OwnerId != ctx.User.Id)
        {
            await ctx.Connection.SendMessageAsync(
                new AlertMessage("You are not the owner of this room")
            );
            return;
        }

        foreach(var connection in RoomUtilities.GetUsersInRoom(ctx, room))
        {
            connection.Metadata.Remove(room);
            await connection.SendMessageAsync(new SendToHotelViewMessage());
        }

        ctx.Database.Rooms.Remove(room);
        await ctx.Database.SaveChangesAsync();
    }
}
