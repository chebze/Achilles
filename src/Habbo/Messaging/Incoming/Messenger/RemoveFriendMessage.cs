using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Outgoing.Messenger;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Messenger;

[IncomingMessage(40)]
public class RemoveFriendMessage : IncomingMessage
{
    public List<int> UserIds { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public RemoveFriendMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }   

    protected override void Parse(IncomingMessageContent content)
    {
        this.UserIds = new List<int>();
        int count = content.ReadWiredInt(); 

        for(int i = 0; i < count; i++)
        {
            this.UserIds.Add(content.ReadWiredInt());
        }
    }
    
    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return;

        foreach(var userId in this.UserIds)
        {
            var friendship = await ctx.Database.UserFriendships
                .FirstOrDefaultAsync(f =>
                (
                    (f.FromUserId == ctx.User.Id && f.ToUserId == userId) ||
                    (f.ToUserId == ctx.User.Id && f.FromUserId == userId)
                ) &&
                f.IsAccepted == true
            );

            if(friendship is null)
                continue;

            var friend = await ctx.Database.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if(friend is null)
                continue;

            ctx.Database.UserFriendships.Remove(friendship);
            await ctx.Database.SaveChangesAsync();
            
            IConnection? friendConnection = UserUtilities.GetConnection(ctx, friend);
            if(friendConnection is not null)
                await friendConnection.SendMessageAsync(
                    new FriendsRemovedMessage([ ctx.User.Id ])
                );
        }

        
        await ctx.Connection.SendMessageAsync(
            new FriendsRemovedMessage(this.UserIds)
        );
    }
}
