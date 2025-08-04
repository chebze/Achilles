using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Messenger;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Messenger;

[IncomingMessage(41)]
public class MessengerFindUserMessage : IncomingMessage
{
    public string Username { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public MessengerFindUserMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.Username = content.ReadString();
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if (ctx.User is null)
            return new MessengerFindUserResponseMessage(ctx, (null, (null, null)));

        Database.Models.User? user = await ctx.Database.Users
            .Where(u => u.Username.ToLower().Contains(this.Username.ToLower()))
        .FirstOrDefaultAsync();

        Room? room = null;
        RoomCategory? category = null;

        if(user is not null)
        {
            IConnection? userConnection = ctx.ConnectionManager.Connections.FirstOrDefault(c => c.Metadata.OfType<Database.Models.User>().Any(u => u.Id == user.Id));
            if (userConnection is not null)
            {
                room = userConnection.Metadata.OfType<Room>().FirstOrDefault();

                if (room is not null)
                    category = await ctx.Database.RoomCategories.FirstOrDefaultAsync(c => c.Id == room.RoomCategoryId);
            }
        }

        return new MessengerFindUserResponseMessage(ctx, (user, (room, category)));
    }

}
