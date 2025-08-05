using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Achilles.Habbo.Messaging.Outgoing.Handshake;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Utilities;

public static class UserUtilities
{
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
            new FuseRightsMessage(ctx)
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
