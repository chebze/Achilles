using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Database.Models;

namespace Achilles.Habbo.Messaging.Outgoing.Messenger;

public class MessengerFindUserResponseMessage : OutgoingMessage
{
    public MessengerFindUserResponseMessage(
        IncomingMessageContext context,
        (Database.Models.User? user, (Room? room, RoomCategory? category) roomInfo) result
    ) : base(128)
    {
        this.AppendString("MESSENGER");

        if(result.user is null)
        {
            this.AppendBoolean(false);
            return;
        }

        bool isOnline = result.user.LastOnline > DateTime.Now.AddMinutes(-5);

        this.AppendWired(result.user.Id);
        this.AppendString(result.user.Username);
        this.AppendBoolean(result.user.Gender == 'M');
        this.AppendString(result.user.ConsoleMotto);

        string roomName = "Hotel View";

        if(isOnline)
        {
            if(result.roomInfo.room is not null && result.roomInfo.category is not null)
            {
                if(result.roomInfo.category.IsForPublicSpaces)
                    roomName = result.roomInfo.room.Name;
                else
                    roomName = "Floor1a";
            }
        }

        this.AppendString(roomName);
        this.AppendString(result.user.LastOnline?.ToString("dd-MM-yyyy HH:mm:ss") ?? "Never");
        this.AppendString(result.user.Figure);
    }
}

