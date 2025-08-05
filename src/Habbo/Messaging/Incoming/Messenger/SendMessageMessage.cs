using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Outgoing.Messenger;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Messenger;

[IncomingMessage(33)]
public class SendMessageMessage : IncomingMessage
{
    public List<int> RecipientIds { get; set; }
    public string Message { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SendMessageMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        int recipientCount = content.ReadWiredInt();
        this.RecipientIds = new List<int>();

        for(int i = 0; i < recipientCount; i++)
        {
            this.RecipientIds.Add(content.ReadWiredInt());
        }

        this.Message = content.ReadString();
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return;
        
        List<MessageReceivedMessage> messages = new List<MessageReceivedMessage>();
        foreach(var recipientId in this.RecipientIds)
        {
            var recipient = await ctx.Database.Users.FirstOrDefaultAsync(u => u.Id == recipientId);
            if(recipient is null)
                continue;

            UserMessage message = new UserMessage()
            {
                FromUserId = ctx.User.Id,
                ToUserId = recipient.Id,
                Message = this.Message,
                SentAt = DateTime.Now
            };
            await ctx.Database.UserMessages.AddAsync(message);
            await ctx.Database.SaveChangesAsync();

            IConnection? recipientConnection = UserUtilities.GetConnection(ctx, recipient);
            if(recipientConnection is null)
                continue;
            
            await recipientConnection.SendMessageAsync(
                new MessageReceivedMessage(
                    message.Id,
                    message.ToUserId,
                    message.SentAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    message.Message
                )
            );
        }
    }
}
