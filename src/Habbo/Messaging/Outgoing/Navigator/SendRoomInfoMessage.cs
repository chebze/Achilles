using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Database.Models;
using Achilles.Habbo.Utilities;

namespace Achilles.Habbo.Messaging.Outgoing.Navigator;

public class SendRoomInfoMessage : OutgoingMessage
{
    public SendRoomInfoMessage(IncomingMessageContext ctx, Room room) : base(54)
    {
        if(ctx.User is null)
            return;

        var owner = ctx.Database.Users.Find(room.OwnerId);
        if(owner is null)
            return;

        var category = ctx.Database.RoomCategories.Find(room.RoomCategoryId);

        int visitors = RoomUtilities.GetUserCountInRoom(ctx, room);
        int maxVisitors = Math.Min(room.MaxVisitors, category?.MaxVisitors ?? 0);

        string ownerName = owner.Username;
        if(!room.ShowOwnerName && ctx.User.Id != owner.Id)
            ownerName = "-";

        this.AppendBoolean(room.AllSuperUsers);
        this.AppendWired((int) room.AccessType);
        this.AppendWired(room.Id);
        this.AppendString(ownerName);
        this.AppendString(room.RoomModelId);
        this.AppendString(room.Name);
        this.AppendString(room.Description);
        this.AppendBoolean(room.ShowOwnerName);
        this.AppendBoolean(category?.AllowTrading ?? false);
        this.AppendBoolean(category == null);
        this.AppendWired(visitors.ToString());
        this.AppendWired(maxVisitors.ToString());
    }
}

