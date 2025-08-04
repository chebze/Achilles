using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(153)]
public class SetRoomCategoryMessage : IncomingMessage
{
    public int RoomId { get; set; }
    public int CategoryId { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SetRoomCategoryMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.RoomId = content.ReadWiredInt();
        this.CategoryId = content.ReadWiredInt();
    }
    
    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return;

        Room? room = await ctx.Database.Rooms.FirstOrDefaultAsync(x => x.Id == this.RoomId);
        if(room is null)
            return;

        RoomCategory? category = await ctx.Database.RoomCategories.FirstOrDefaultAsync(x => x.Id == this.CategoryId);
        if(category is null)
        {
            await ctx.Connection.SendMessageAsync(
                new AlertMessage($"Category #{this.CategoryId} not found")
            );
            return;
        }

        if(room.OwnerId != ctx.User.Id)
        {
            await ctx.Connection.SendMessageAsync(
                new AlertMessage("You are not the owner of this room")
            );
            return;
        }

        room.RoomCategoryId = this.CategoryId;
        await ctx.Database.SaveChangesAsync();
    }
    
}

