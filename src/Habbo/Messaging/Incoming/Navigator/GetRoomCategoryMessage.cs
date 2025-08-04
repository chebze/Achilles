using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Achilles.Habbo.Messaging.Outgoing.Navigator;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(152)]
public class GetRoomCategoryMessage : IncomingMessage
{
    public int RoomId { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public GetRoomCategoryMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.RoomId = content.ReadWiredInt();
    }
    
    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return null;

        Room? room = await ctx.Database.Rooms.FirstOrDefaultAsync(x => x.Id == this.RoomId);
        if(room is null)
            return new AlertMessage($"Room #{this.RoomId} not found");

        return new GetRoomCategoryResponseMessage(ctx, room);
    }
    
}

