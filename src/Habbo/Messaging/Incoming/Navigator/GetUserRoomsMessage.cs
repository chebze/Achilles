using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Navigator;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(16)]
public class GetUserRoomsMessage : IncomingMessage
{
    public GetUserRoomsMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return null;
            
        List<Room> rooms = await ctx.Database.Rooms
            .Where(x => x.OwnerId == ctx.User.Id)
            .ToListAsync();

        if(rooms.Count == 0)
            return new NoRoomsFoundForUserMessage(ctx.User!.Username);
        
        return new SendUserRoomsMessage(ctx, rooms);
    }
}
