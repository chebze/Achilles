namespace Achilles.Habbo.Messaging.Incoming.Messenger;

using System.Runtime.CompilerServices;
using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Messenger;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;
using static Achilles.Habbo.Messaging.Outgoing.Messenger.MessengerErrorResponse;

[IncomingMessage(39)]
public class SendFriendRequestMessage : IncomingMessage
{
    public string Username { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SendFriendRequestMessage(MessageHeader header, string content) : base(header, content)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.Username = content.ReadString();
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext context)
    {
        if(context.User is null)
            return null;

        Database.Models.User? otherUser = await context.Database.Users
            .FirstOrDefaultAsync(u => u.Username == this.Username);

        if(otherUser is null)
            return new MessengerErrorResponse(context, MessengerErrorType.FriendRequestNotFound);

        if(otherUser.Id == context.User.Id)
            return new MessengerErrorResponse(context, MessengerErrorType.FriendRequestNotFound);

        bool alreadyFriends = await context.Database.UserFriendships
            .AnyAsync(c => (c.FromUserId == context.User.Id && c.ToUserId == otherUser.Id) || (c.FromUserId == otherUser.Id && c.ToUserId == context.User.Id));

        if(alreadyFriends)
            return new MessengerErrorResponse(context, MessengerErrorType.FriendRequestNotFound);

        int myFriendLimit = context.User.IsClubMember ? context.Configuration.Messenger.ClubFriendLimit : context.Configuration.Messenger.ClubFriendLimit;
        int myFriendCount = await context.Database.UserFriendships
            .CountAsync(c => (c.FromUserId == context.User.Id || c.ToUserId == context.User.Id) && c.IsAccepted);

        int otherFriendLimit = otherUser.IsClubMember ? context.Configuration.Messenger.ClubFriendLimit : context.Configuration.Messenger.ClubFriendLimit;
        int otherFriendCount = await context.Database.UserFriendships
            .CountAsync(c => (c.FromUserId == otherUser.Id || c.ToUserId == otherUser.Id) && c.IsAccepted);

        if(myFriendCount >= myFriendLimit)
            return new MessengerErrorResponse(context, MessengerErrorType.FriendsListFull);
        if(otherFriendCount >= otherFriendLimit)
            return new MessengerErrorResponse(context, MessengerErrorType.TargetFriendsListFull);

        if(!otherUser.AllowFriendRequests)
            return new MessengerErrorResponse(context, MessengerErrorType.TargetDoesNotAccept);

        UserFriendship? friendship = new UserFriendship()
        {
            FromUserId = context.User.Id,
            ToUserId = otherUser.Id,
            IsAccepted = false,
            SentAt = DateTime.Now
        };

        context.Database.UserFriendships.Add(friendship);
        await context.Database.SaveChangesAsync();

        if(UserUtilities.GetConnection(context, otherUser) is IConnection otherUserConnection && otherUserConnection is not null)
        {
            await otherUserConnection.SendMessageAsync(
                new GetFriendRequestsResponseMessage(
                    otherUser.Id,
                    otherUser.Username
                )
            );
        }
        
        return null;
    }
}
