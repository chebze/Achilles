using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Achilles.Habbo.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.User;

[IncomingMessage(204)]
public class TrySSOLoginMessage : IncomingMessage
{
    public string Ticket { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public TrySSOLoginMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.Ticket = content.ReadString();
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if(string.IsNullOrEmpty(this.Ticket))
            return new LocalizedErrorMessage("Login incorrect");

        Database.Models.User? user;
        if(this.Ticket.StartsWith("auth_"))
        {
            string[] parts = this.Ticket.Split('_');
            if(parts.Length != 3)
                return new LocalizedErrorMessage("Login incorrect");

            user = await ctx.Database.Users.FirstOrDefaultAsync(u => u.Username == parts[1] && u.Password == parts[2]);
        }
        else
            user = await ctx.Database.Users.FirstOrDefaultAsync(u => u.SSOTicket == this.Ticket);

        if(user is null)
            return new LocalizedErrorMessage("Login incorrect");

        return await UserUtilities.HandleUserLogin(ctx, user);
    }
}