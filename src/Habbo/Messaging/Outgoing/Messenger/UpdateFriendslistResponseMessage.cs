using System.Data.Common;
using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.Messenger;

public class UpdateFriendslistResponseMessage : OutgoingMessage
{
    public UpdateFriendslistResponseMessage(
        IncomingMessageContext context,
        List<(UserFriendship friendship, Database.Models.User otherUser, (Room? room, RoomCategory? category) roomInfo)> friendships
    ) : base(13)
    {
        this.AppendWired(friendships.Count);

        foreach(var (friendship, otherUser, roomInfo) in friendships)
        {
            bool isOnline = otherUser.LastOnline > DateTime.Now.AddMinutes(-5);

            this.AppendWired(friendship.Id);
            this.AppendString(otherUser.Username);
            this.AppendBoolean(otherUser.Gender == 'M');
            this.AppendString(otherUser.ConsoleMotto);

            string roomName = "Hotel view";

            if(isOnline)
            {
                if(roomInfo.room is not null && roomInfo.category is not null)
                {
                    if(roomInfo.category.IsForPublicSpaces)
                        roomName = roomInfo.room.Name;
                    else
                        roomName = "Floor1a";
                }
            }

            this.AppendString(roomName);
            this.AppendChar((char)2);
            this.AppendString(otherUser.LastOnline?.ToString("dd-MM-yyyy HH:mm:ss") ?? "Never");
            this.AppendString(otherUser.Figure);
        }
    }
}