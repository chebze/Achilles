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

[IncomingMessage(37)]
public class AcceptFriendRequestMessage : IncomingMessage
{
    public int UserId { get; private set; }

    public AcceptFriendRequestMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.UserId = content.ReadWiredInt();
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if (ctx.User is null)
            return null;

        UserFriendship? friendship = await ctx.Database.UserFriendships
            .Where(f => f.ToUserId == ctx.User.Id && f.FromUserId == this.UserId && f.IsAccepted == false)
            .FirstOrDefaultAsync();

        if (friendship is null)
        {
            return new FriendRequestResultMessage(
                ctx,
                [
                    (ctx.User.Username, MessengerErrorType.FriendRequestNotFound)
                ]
            );
        }

        Database.Models.User? otherUser = await ctx.Database.Users
            .FirstOrDefaultAsync(u => u.Id == this.UserId);

        if (otherUser is null)
        {
            ctx.Database.UserFriendships.Remove(friendship);
            await ctx.Database.SaveChangesAsync();
            return new FriendRequestResultMessage(
                ctx,
                [
                    (ctx.User.Username, MessengerErrorType.FriendRequestNotFound)
                ]
            );
        }

        if (otherUser.Id == ctx.User.Id)
        {
            ctx.Database.UserFriendships.Remove(friendship);
            await ctx.Database.SaveChangesAsync();
            return new FriendRequestResultMessage(
                ctx,
                [
                    (ctx.User.Username, MessengerErrorType.FriendRequestNotFound)
                ]
            );
        }

        bool alreadyFriends = await ctx.Database.UserFriendships
            .AnyAsync(c => ((c.FromUserId == ctx.User.Id && c.ToUserId == otherUser.Id) || (c.FromUserId == otherUser.Id && c.ToUserId == ctx.User.Id)) && c.IsAccepted);
        if (alreadyFriends)
        {
            ctx.Database.UserFriendships.Remove(friendship);
            await ctx.Database.SaveChangesAsync();
            return new FriendRequestResultMessage(
                ctx,
                [
                    (ctx.User.Username, MessengerErrorType.FriendRequestNotFound)
                ]
            );
        }

        int myFriendLimit = ctx.User.IsClubMember ? ctx.Configuration.Messenger.ClubFriendLimit : ctx.Configuration.Messenger.ClubFriendLimit;
        int myFriendCount = await ctx.Database.UserFriendships
            .CountAsync(c => (c.FromUserId == ctx.User.Id || c.ToUserId == ctx.User.Id) && c.IsAccepted);

        int otherFriendLimit = otherUser.IsClubMember ? ctx.Configuration.Messenger.ClubFriendLimit : ctx.Configuration.Messenger.ClubFriendLimit;
        int otherFriendCount = await ctx.Database.UserFriendships
            .CountAsync(c => (c.FromUserId == otherUser.Id || c.ToUserId == otherUser.Id) && c.IsAccepted);

        if (myFriendCount >= myFriendLimit)
        {
            ctx.Database.UserFriendships.Remove(friendship);
            await ctx.Database.SaveChangesAsync();
            return new FriendRequestResultMessage(
                ctx,
                [
                    (ctx.User.Username, MessengerErrorType.FriendsListFull)
                ]
            );
        }

        if (otherFriendCount >= otherFriendLimit)
        {
            ctx.Database.UserFriendships.Remove(friendship);
            await ctx.Database.SaveChangesAsync();
            return new FriendRequestResultMessage(
                ctx,
                [
                    (otherUser.Username, MessengerErrorType.TargetFriendsListFull)
                ]
            );
        }

        if (!otherUser.AllowFriendRequests)
        {
            ctx.Database.UserFriendships.Remove(friendship);
            await ctx.Database.SaveChangesAsync();
            return new FriendRequestResultMessage(
                ctx,
                [
                    (otherUser.Username, MessengerErrorType.TargetDoesNotAccept)
                ]
            );
        }

        friendship.IsAccepted = true;
        friendship.AcceptedAt = DateTime.Now;
        friendship.IsDeclined = false;
        friendship.DeclinedAt = null;
        await ctx.Database.SaveChangesAsync();

        Room? otherUserRoom = ctx.Room;
        RoomCategory? otherUserRoomCategory = null;

        if (otherUserRoom is not null)
            otherUserRoomCategory = await ctx.Database.RoomCategories.FirstOrDefaultAsync(c => c.Id == otherUserRoom.RoomCategoryId);
        
        // inform the current user client about the new friend
        await ctx.Connection.SendMessageAsync(
            new FriendRequestAcceptedMessage(
                ctx,
                otherUser,
                otherUserRoom,
                otherUserRoomCategory
            )
        );

        // inform the other user client about the current user
        if (UserUtilities.GetConnection(ctx, otherUser) is IConnection otherUserConnection)
        {
            RoomCategory? currentUserRoomCategory = null;

            if (ctx.Room is not null)
                currentUserRoomCategory = await ctx.Database.RoomCategories.FirstOrDefaultAsync(c => c.Id == ctx.Room.RoomCategoryId);

            await otherUserConnection.SendMessageAsync(
                new FriendRequestAcceptedMessage(
                    ctx,
                    ctx.User,
                    ctx.Room,
                    currentUserRoomCategory
                )
            );
        }

        return new FriendRequestResultMessage(ctx, []);
    }
}
