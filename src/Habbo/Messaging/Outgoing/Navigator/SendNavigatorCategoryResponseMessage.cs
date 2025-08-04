using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Utilities;

namespace Achilles.Habbo.Messaging.Outgoing.Navigator;

public class SendNavigatorCategoryResponseMessage : OutgoingMessage
{
    public SendNavigatorCategoryResponseMessage(
        IncomingMessageContext ctx,
        RoomCategory category,
        List<RoomCategory> subcategories,
        List<Room> rooms,
        bool HideFullRooms,
        int CategoryCurrentVisitors,
        int CategoryMaxVisitors
    ) : base(220)
    {
        if (category is null)
            return;

        this.AppendBoolean(HideFullRooms);
        this.AppendWired(category.Id);
        this.AppendWired(category.IsForPublicSpaces ? 0 : 2);
        this.AppendString(category.Name);
        this.AppendWired(CategoryCurrentVisitors);
        this.AppendWired(CategoryMaxVisitors);
        this.AppendWired(category?.ParentId ?? 0);
        if(!category!.IsForPublicSpaces)
            this.AppendWired(rooms.Count());

        foreach (var room in rooms)
        {
            int visitors = RoomUtilities.GetUserCountInRoom(ctx, room);
            int maxVisitors = Math.Min(room.MaxVisitors, CategoryMaxVisitors);
            if (HideFullRooms && visitors >= maxVisitors)
                continue;

            string ownerName = "-";
            if(!room.ShowOwnerName && ctx.User?.Id != room.OwnerId)
            {
                var owner = ctx.Database.Users.Find(room.OwnerId);
                if(owner is not null)
                    ownerName = owner.Username;
            }

            this.AppendWired(room.Id);
            if(category!.IsForPublicSpaces)
                this.AppendWired(1);
            this.AppendString(room.Name);
            this.AppendString(ownerName);
            this.AppendString(room.AccessType.ToString().ToLower());

            this.AppendWired(visitors);
            this.AppendWired(maxVisitors);

            if(category.IsForPublicSpaces)
                this.AppendWired(category.Id);
                
            this.AppendString(room.Description);
            
            if(!category.IsForPublicSpaces)
            {
                this.AppendWired(room.Id);
                this.AppendBoolean(false);
                this.AppendString(room.CCTs!);
                this.AppendBoolean(false);
                this.AppendBoolean(true);
            }
        }

        foreach(var subcategory in subcategories)
        {
            if(subcategory.ParentId is null)
                continue;
            if(subcategory.Id == category!.Id)
                continue;
            
            int visitors = RoomUtilities.GetUserCountInCategoryRooms(ctx, subcategory);
            if(HideFullRooms && visitors >= subcategory.MaxVisitors)
                continue;

            this.AppendWired(subcategory.Id);
            this.AppendWired(0);
            this.AppendString(subcategory.Name);
            this.AppendWired(visitors);
            this.AppendWired(subcategory.MaxVisitors);
            this.AppendWired(subcategory.ParentId);
        }
    }
}   

