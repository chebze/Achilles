using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Navigator;

public class SendUserFavouriteRoomsMessage : OutgoingMessage
{
    public SendUserFavouriteRoomsMessage(IncomingMessageContext ctx, List<Room> rooms) : base(61)
    {
    }
}