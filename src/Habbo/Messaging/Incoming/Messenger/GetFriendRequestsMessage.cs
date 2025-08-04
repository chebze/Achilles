using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Messenger;
using Achilles.Habbo.Messaging.Outgoing.Recycler;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.User;

[IncomingMessage(233)]
public class GetFriendRequestsMessage : IncomingMessage
{
    public GetFriendRequestsMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override async Task HandleAsync(IncomingMessageContext context)
    {
        if(context.User is null)
            return;

        List<UserFriendship> friendRequests = await context.Database.UserFriendships
            .Where(c => c.ToUserId == context.User.Id && c.IsAccepted == false && c.IsDeclined == false)
            .ToListAsync();
            
        foreach(UserFriendship friendship in friendRequests)
        {
            Database.Models.User? fromUser = await context.Database.Users.FirstOrDefaultAsync(c => c.Id == friendship.FromUserId);
            if(fromUser is null)
                continue;

            await context.Connection.SendMessageAsync(
                new GetFriendRequestsResponseMessage(
                    fromUser.Id,
                    fromUser.Username
                )
            );
        }
    }
}