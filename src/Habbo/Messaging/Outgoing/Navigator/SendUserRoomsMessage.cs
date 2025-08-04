using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Database.Models;
using Achilles.Habbo.Utilities;

namespace Achilles.Habbo.Messaging.Outgoing.Navigator;

public class SendUserRoomsMessage : OutgoingMessage
{
    public SendUserRoomsMessage(IncomingMessageContext ctx, List<Room> rooms) : base(16)
    {
        foreach(var room in rooms)
        {
            var category = ctx.Database.RoomCategories.Find(room.RoomCategoryId);
            if(category is null)
                continue;

            var owner = ctx.Database.Users.Find(room.OwnerId);
            if(owner is null)
                continue;

            string ownerName = owner.Username;
            if(!room.ShowOwnerName && ctx.User?.Id != owner.Id)
                ownerName = "-";

            int visitors = RoomUtilities.GetUserCountInRoom(ctx, room);
            int maxVisitors = Math.Min(room.MaxVisitors, category.MaxVisitors);

            this.AppendString(room.Id, (char) 9);
            this.AppendString(room.Name, (char) 9);
            this.AppendString(ownerName, (char) 9);
            this.AppendString(room.AccessType.ToString().ToLower(), (char) 9);
            this.AppendString("x", (char) 9);
            this.AppendString(visitors.ToString(), (char) 9);
            this.AppendString(maxVisitors.ToString(), (char) 9);
            this.AppendString("null", (char) 9);
            this.AppendString(room.Description, (char) 9);
            this.AppendChar((char) 13);
        }
    }
}

