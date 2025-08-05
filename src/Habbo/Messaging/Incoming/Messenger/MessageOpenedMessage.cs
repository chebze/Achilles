using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Messenger;

[IncomingMessage(32)]
public class MessageOpenedMessage : IncomingMessage
{
    public int MessageId { get; set; }
    
    public MessageOpenedMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.MessageId = content.ReadWiredInt();
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return;

        var message = await ctx.Database.UserMessages
            .FirstOrDefaultAsync(m => m.Id == this.MessageId && m.ToUserId == ctx.User.Id);

        if(message is null)
            return;

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;

        await ctx.Database.SaveChangesAsync();
    }
}
