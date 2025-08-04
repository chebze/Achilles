using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(20)]
public class DeleteFavouriteRoomMessage : IncomingMessage
{
    public int RoomType { get; private set; }
    public int RoomId { get; private set; }

    public DeleteFavouriteRoomMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.RoomType = content.ReadWiredInt();
        this.RoomId = content.ReadWiredInt();
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return;

        var favouriteRoom = await ctx.Database.UserFavouriteRooms
            .FirstOrDefaultAsync(x => x.UserId == ctx.User.Id && x.RoomId == this.RoomId);

        if(favouriteRoom is null)
            return;

        ctx.Database.UserFavouriteRooms.Remove(favouriteRoom);
        await ctx.Database.SaveChangesAsync();
        
        return;
    }
}

