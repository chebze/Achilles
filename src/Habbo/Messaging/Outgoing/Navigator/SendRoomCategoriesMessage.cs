using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Database.Models;

namespace Achilles.Habbo.Messaging.Outgoing.Navigator;

public class SendRoomCategoriesMessage : OutgoingMessage
{
    public SendRoomCategoriesMessage(IncomingMessageContext ctx, List<RoomCategory> categories) : base(221)
    {
        this.AppendWired(categories.Count);
        foreach(var category in categories)
        {
            this.AppendWired(category.Id);
            this.AppendString(category.Name);
        }
    }
}

