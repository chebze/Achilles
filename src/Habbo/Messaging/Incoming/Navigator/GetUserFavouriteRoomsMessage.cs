using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Navigator;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(18)]
public class GetUserFavouriteRoomsMessage : IncomingMessage
{
    public GetUserFavouriteRoomsMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return null;

        List<int> favouriteRooms = await ctx.Database.UserFavouriteRooms
            .Where(x => x.UserId == ctx.User.Id)
            .Select(x => x.RoomId)
            .ToListAsync();
        List<Room> rooms = await ctx.Database.Rooms
            .Where(x => favouriteRooms.Contains(x.Id))
            .ToListAsync();

        return new SendUserFavouriteRoomsMessage(ctx, rooms);
    }
}
