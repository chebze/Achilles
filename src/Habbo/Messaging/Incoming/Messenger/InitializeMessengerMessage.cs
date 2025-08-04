using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Messenger;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.User;

[IncomingMessage(12)]
public class InitializeMessengerMessage : IncomingMessage
{
    public InitializeMessengerMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return new InitializeMessengerResponseMessage(ctx, []);

        List<UserFriendship> friendships = await ctx.Database.UserFriendships
            .Where(f => (f.ToUserId == ctx.User.Id || f.FromUserId == ctx.User.Id) && f.IsAccepted)
            .ToListAsync();

        List<(UserFriendship friendship, Database.Models.User otherUser, (Room? room, RoomCategory? category) roomInfo)> responseData = [];

        foreach(var friendship in friendships)
        {
            int friendId = friendship.FromUserId == ctx.User.Id ? friendship.ToUserId : friendship.FromUserId;
            if(await ctx.Database.Users.FirstOrDefaultAsync(u => u.Id == friendId) is Database.Models.User otherUser && otherUser is not null)
            {
                Room? room = null;
                RoomCategory? category = null;

                IConnection? otherUserConnection = ctx.ConnectionManager.Connections.FirstOrDefault(c => c.Metadata.OfType<Database.Models.User>().Any(u => u.Id == otherUser.Id));
                if(otherUserConnection is not null)
                {
                    room = otherUserConnection.Metadata.OfType<Room>().FirstOrDefault();

                    if(room is not null)
                        category = await ctx.Database.RoomCategories.FirstOrDefaultAsync(c => c.Id == room.RoomCategoryId);
                }

                responseData.Add((friendship, otherUser, (room, category)));
            }else continue;
        }
        return new InitializeMessengerResponseMessage(ctx, responseData);
    }
}