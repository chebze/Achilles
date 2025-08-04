using Achilles.Habbo.Configuration;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Achilles.Habbo.Messaging.Outgoing.Handshake;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.User;

[IncomingMessage(4)]
public class TryLoginMessage : IncomingMessage
{
    public string Username { get; private set; }
    public string Password { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public TryLoginMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.Username = content.ReadString();
        this.Password = content.ReadString();
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        var user = await ctx.Database.Users.FirstOrDefaultAsync(u =>
            u.Username == this.Username &&
            u.Password == this.Password
        );

        if (user is null)
            return new LocalizedErrorMessage("Login incorrect");

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

        ctx.Logger.LogInformation("User {Username} logged in", this.Username);

        return new LoginMessage();
    }
}
