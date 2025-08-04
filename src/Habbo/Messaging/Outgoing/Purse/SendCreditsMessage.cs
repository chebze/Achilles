using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.Purse;

public class SendCreditsMessage : OutgoingMessage
{
    public SendCreditsMessage(int credits) : base(6)
    {
        this.Build(credits);
    }
    public SendCreditsMessage(IncomingMessageContext ctx) : base(6)
    {
        this.Build(ctx.User?.Credits ?? 0);
    }

    private void Build(int credits)
    {
        this.AppendString(credits.ToString() + ".0");
    }
}
