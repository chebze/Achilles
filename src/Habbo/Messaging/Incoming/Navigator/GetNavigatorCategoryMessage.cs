using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Navigator;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(150)]
public class GetNavigatorCategoryMessage : IncomingMessage
{
    public bool HideFull { get; set; }
    public int CategoryId { get; set; }

    public GetNavigatorCategoryMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.HideFull = content.ReadBase64Boolean();
        this.CategoryId = content.ReadWiredInt();
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        RoomCategory? category = await ctx.Database.RoomCategories.FindAsync(this.CategoryId);

        if (category == null)
            return null;

        List<RoomCategory> subcategories = await ctx.Database.RoomCategories.Where(c => c.ParentId == category.Id).ToListAsync();
        List<Room> rooms = await ctx.Database.Rooms.Where(r => r.RoomCategoryId == category.Id).ToListAsync();

        int currentVisitors = RoomUtilities.GetUserCountInCategoryRooms(ctx, category);
        if(HideFull)
            rooms.RemoveAll(room => room.MaxVisitors >= RoomUtilities.GetUserCountInRoom(ctx, room));

        return new SendNavigatorCategoryResponseMessage(ctx, category, subcategories, rooms, this.HideFull, currentVisitors, category.MaxVisitors);
    }
}
