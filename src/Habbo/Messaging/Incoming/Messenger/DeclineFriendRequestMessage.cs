using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Messenger;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;
using static Achilles.Habbo.Messaging.Outgoing.Messenger.MessengerErrorResponse;

namespace Achilles.Habbo.Messaging.Incoming.Messenger;

[IncomingMessage(38)]
public class DeclineFriendRequestMessage : IncomingMessage
{
    public bool DeclineAll { get; private set; }
    public List<int> UserIds { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public DeclineFriendRequestMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.DeclineAll = content.ReadWiredBoolean();
        int count = content.ReadWiredInt();
        for (int i = 0; i < count; i++)
        {
            this.UserIds.Add(content.ReadWiredInt());
        }
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if (ctx.User is null)
            return;

        if (this.DeclineAll)
        {
            List<UserFriendship> friendships = await ctx.Database.UserFriendships
                .Where(f => f.ToUserId == ctx.User.Id && f.IsAccepted == false)
                .ToListAsync();

            foreach (UserFriendship friendship in friendships)
            {
                friendship.IsAccepted = false;
                friendship.AcceptedAt = null;
                friendship.IsDeclined = true;
                friendship.DeclinedAt = DateTime.Now;
            }
        }
        else
        {
            foreach (int userId in this.UserIds)
            {
                UserFriendship? friendship = await ctx.Database.UserFriendships
                    .Where(f => f.ToUserId == ctx.User.Id && f.FromUserId == userId && f.IsAccepted == false && f.IsDeclined == false)
                .FirstOrDefaultAsync();

                if (friendship is null)
                    continue;


                friendship.IsAccepted = false;
                friendship.AcceptedAt = null;
                friendship.IsDeclined = true;
                friendship.DeclinedAt = DateTime.Now;
            }
        }

        await ctx.Database.SaveChangesAsync();
    }
}
