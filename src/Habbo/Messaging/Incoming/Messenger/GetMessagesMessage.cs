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

[IncomingMessage(191)]
public class GetMessagesMessage : IncomingMessage
{
    public GetMessagesMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return;

        List<UserMessage> messages = await ctx.Database.UserMessages
            .Where(m => m.ToUserId == ctx.User.Id && m.IsRead == false)
        .ToListAsync();

        foreach(var message in messages)
        {
            await ctx.Connection.SendMessageAsync(
                new MessageReceivedMessage(
                    message.Id,
                    message.ToUserId,
                    message.SentAt.ToString("dd-MM-yyyy HH:mm:ss"),
                    message.Message
                )
            );
        }
    }
}