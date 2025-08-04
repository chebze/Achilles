using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(19)]
public class AddFavouriteRoomMessage : IncomingMessage
{
    public int RoomType { get; private set; }
    public int RoomId { get; private set; }

    public AddFavouriteRoomMessage(MessageHeader header, string raw) : base(header, raw)
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

        bool alreadyFavourited = await ctx.Database.UserFavouriteRooms
            .AnyAsync(x => x.UserId == ctx.User.Id && x.RoomId == this.RoomId);

        if(alreadyFavourited)
            return;

        ctx.Database.UserFavouriteRooms.Add(new UserFavouriteRoom
        {
            UserId = ctx.User.Id,
            RoomId = this.RoomId
        });

        await ctx.Database.SaveChangesAsync();
        return;
    }
}

