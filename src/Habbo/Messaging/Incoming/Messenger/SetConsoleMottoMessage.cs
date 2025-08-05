using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;

namespace Achilles.Habbo.Messaging.Incoming.Messenger;

[IncomingMessage(36)]
public class SetConsoleMottoMessage : IncomingMessage
{
    public string Motto { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SetConsoleMottoMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.Motto = content.ReadString();
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return;

        ctx.User.ConsoleMotto = this.Motto;
        await ctx.Database.SaveChangesAsync();
    }
}
