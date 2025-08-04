using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Incoming.User;

[IncomingMessage(196)]
public class PongMessage : IncomingMessage
{
    public PongMessage(MessageHeader header, string content) : base(header, content)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if (ctx.PingState.LastPing is null)
            ctx.PingState.LastPing = DateTime.Now.AddSeconds(-1);

        ctx.PingState.LastPong = DateTime.Now;
        
        ctx.Logger
            .LogInformation(
                "Connection {id} responded to ping in {time}ms",
                ctx.Connection.Id,
                (DateTime.Now - ctx.PingState.LastPing).Value.TotalMilliseconds
            );

        ctx.PingState.LastPing = null;

        if(ctx.User is var user && user is not null)
        {
            user.LastOnline = DateTime.Now;
            await ctx.Database.SaveChangesAsync();
        }
    }
}

