using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Messenger;

public class FriendRequestAcceptedMessage : OutgoingMessage
{
    public FriendRequestAcceptedMessage(
        IncomingMessageContext context,
        Database.Models.User friend,
        Room? friendRoom,
        RoomCategory? friendRoomCategory
    ) : base(137)
    {

        bool isOnline = friend.LastOnline > DateTime.Now.AddMinutes(-5);

        this.AppendWired(friend.Id);
        this.AppendString(friend.Username);
        this.AppendBoolean(friend.Gender == 'M');
        this.AppendString(friend.ConsoleMotto);

        string roomName = "Hotel View";

        if (isOnline)
        {
            if (friendRoom is not null && friendRoomCategory is not null)
            {
                if (friendRoomCategory.IsForPublicSpaces)
                    roomName = friendRoom.Name;
                else
                    roomName = "Floor1a";
            }
        }

        this.AppendString(roomName);
        this.AppendChar((char)2);
        this.AppendString(friend.LastOnline?.ToString("dd-MM-yyyy HH:mm:ss") ?? "Never");
        this.AppendString(friend.Figure);
    }
}
