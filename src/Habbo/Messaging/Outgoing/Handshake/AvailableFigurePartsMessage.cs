using Achilles.Habbo.Configuration;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.Handshake;

public class AvailableFigurePartsMessage : OutgoingMessage
{
    public AvailableFigurePartsMessage(string parts) : base(8)
    {
        this.AppendString(parts);
    }
    public AvailableFigurePartsMessage(string[] parts) : base(8)
    {
        this.AppendString("[" + string.Join(",", parts) + "]");
    }
    public AvailableFigurePartsMessage(int[] parts) : base(8)
    {
        this.AppendString("[" + string.Join(",", parts) + "]");
    }
    public AvailableFigurePartsMessage(IncomingMessageContext ctx) : base(8)
    {
        var figureParts = (ctx?.User?.IsClubMember ?? false) ? ctx.Configuration.FigureParts.Club : ctx!.Configuration.FigureParts.Default;
        this.AppendString("[" + string.Join(",", figureParts) + "]");
    }
}
