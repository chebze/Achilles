using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(24)]
public class UpdateRoomMessage : IncomingMessage
{
    public int RoomId { get; set; }
    public string RoomName { get; set; }
    public RoomAccessType AccessType { get; set; }
    public bool ShowOwnerName { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public UpdateRoomMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        string[] data = content.ReadRemainingString().Split('/');
        this.RoomId = int.Parse(data[0]);
        this.RoomName = data[1];
        this.AccessType = Enum.Parse<RoomAccessType>(data[2], true);
        this.ShowOwnerName = data[3] == "1";
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

        room.Name = this.RoomName;
        room.AccessType = this.AccessType;
        room.ShowOwnerName = this.ShowOwnerName;
        await ctx.Database.SaveChangesAsync();
    }
}
