using System.Numerics;
using System.Security.Principal;
using Achilles.Database.Models;
using Achilles.Habbo.Configuration;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Outgoing.Messenger;

public class InitializeMessengerResponseMessage : OutgoingMessage
{
    public InitializeMessengerResponseMessage(
        IncomingMessageContext ctx,
        List<(UserFriendship friendship, Database.Models.User otherUser, (Room? room, RoomCategory? category) roomInfo)> friends
    ) : base(12)
    {
        if(ctx.User is null)
            return;

        this.AppendString(ctx.User.Motto);
        this.AppendWired(ctx.User.IsClubMember ? ctx.Configuration.Messenger.ClubFriendLimit : ctx.Configuration.Messenger.NormalFriendLimit );
        this.AppendWired(ctx.Configuration.Messenger.NormalFriendLimit);
        this.AppendWired(ctx.Configuration.Messenger.ClubFriendLimit);
        this.AppendWired(friends.Count);
        foreach (var (friendship, otherUser, (room, category)) in friends)
        {
            bool isOnline = otherUser.LastOnline > DateTime.Now.AddMinutes(-5);

            this.AppendWired(otherUser.Id);
            this.AppendString(otherUser.Username);
            this.AppendBoolean(otherUser.Gender == 'M');
            this.AppendString(otherUser.ConsoleMotto);

            string roomName = "Hotel View";

            if(isOnline)
            {
                if(room is not null && category is not null)
                {
                    if(category.IsForPublicSpaces)
                        roomName = room.Name;
                    else
                        roomName = "Floor1a";
                }
            }

            this.AppendString(roomName);
            this.AppendString(otherUser.LastOnline?.ToString("dd-MM-yyyy HH:mm:ss") ?? "Never");
            this.AppendString(otherUser.Figure);
        }
    }
}