using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Navigator;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(151)]
public class GetRoomCategoriesMessage : IncomingMessage
{
    public GetRoomCategoriesMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }
    
    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        var categories = await ctx.Database.RoomCategories
            .Where(c => c.IsAssignableToRoom && !c.IsForPublicSpaces)
            .ToListAsync();
            
        return new SendRoomCategoriesMessage(ctx, categories);
    }
}
