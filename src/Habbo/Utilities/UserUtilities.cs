using Achilles.Database.Models;
using Achilles.Habbo.Configuration;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Achilles.Habbo.Messaging.Outgoing.Handshake;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.Migrations;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Utilities;

public static class UserUtilities
{
    public static Task<List<string>> GetFuseRights(IncomingMessageContext ctx)
    {
        List<string> fuseRights = [];

        if(ctx.User is null)
            return Task.FromResult(fuseRights);

        RankConfiguration? rank = ctx.Configuration.Ranks.FirstOrDefault(r => r.Name == ctx.User.Rank);
        if(rank is null)
            return Task.FromResult(fuseRights);

        fuseRights.AddRange(rank.FuseRights);

        if(ctx.User.IsClubMember)
        {
            fuseRights.AddRange([
                "fuse_priority_access",
                "fuse_use_special_room_layouts",
                "fuse_use_club_outfits",
                "fuse_use_club_outfits_default",
                "fuse_use_club_badge",
                "fuse_use_club_dance",
                "fuse_habbo_chooser",
                "fuse_furni_chooser",
                "fuse_extended_buddylist",
                "fuse_room_queue_club"
            ]);
        }

        return Task.FromResult(fuseRights);
    }

    public static IConnection? GetConnection(IncomingMessageContext ctx, User user)
    {
        return ctx.ConnectionManager.Connections
            .Where(c => c.Metadata.OfType<User>().Any(u => u.Id == user.Id))
            .FirstOrDefault();
    }
    
    public static async Task<OutgoingMessage?> HandleUserLogin(IncomingMessageContext ctx, User user)
    {
        var existingConnections = ctx.ConnectionManager.Connections
            .Where(c => c.Metadata.OfType<Database.Models.User>().Any(u => u.Id == user.Id))
            .ToList();
        if(existingConnections.Any())
        {
            foreach(var connection in existingConnections)
            {
                await connection.SendMessageAsync(
                    new LogoutMessage(LogoutMessage.LogoutReason.LogoutConcurrent)
                );
                await connection.CloseAsync();
            }
        }

        // TODO: check if banned



        user.SSOTicket = null;
        user.LastLogin = DateTime.Now;
        user.LastOnline = DateTime.Now;
        user.UpdateClubSubscriptionStatus();

        await ctx.Database.SaveChangesAsync();
        ctx.Connection.Metadata.Add(user);

        await ctx.Connection.SendMessageAsync(
            new FuseRightsMessage(await GetFuseRights(ctx))
        );
        await ctx.Connection.SendMessageAsync(
            new AvailableBadgesMessage(ctx)
        );
        await ctx.Connection.SendMessageAsync(
            new AvailableFigurePartsMessage(ctx)
        );

        // TODO: motd
        // TODO: hotel closed
        // TODO: hc gift due?
        // TODO: send friendslist update to online friends

        ctx.Logger.LogInformation("User {Username} logged in", user.Username);
        return new LoginMessage();
    }
}
