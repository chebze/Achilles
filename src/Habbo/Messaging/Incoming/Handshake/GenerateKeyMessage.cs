using Achilles.Habbo.Configuration;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Handshake;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Incoming.Handshake;

[IncomingMessage(202)]
public class GenerateKeyMessage : IncomingMessage
{
    public GenerateKeyMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        await ctx.Connection.SendMessageAsync(new SessionParametersMessage(new() {
            [SessionParametersMessage.SessionParameterType.EnableCOPPA] = 0,
            [SessionParametersMessage.SessionParameterType.EnableVouchers] = true,
            [SessionParametersMessage.SessionParameterType.PartnerIntegrationEnabled] = false,
            [SessionParametersMessage.SessionParameterType.AllowProfileEditing] = true,
            [SessionParametersMessage.SessionParameterType.DateFormat] = "dd-MM-yyyy",
            [SessionParametersMessage.SessionParameterType.RegisterRequireParentEmail] = false,
            [SessionParametersMessage.SessionParameterType.RegisterSendParentEmail] = false,
            [SessionParametersMessage.SessionParameterType.AllowDirectMail] = false,
            [SessionParametersMessage.SessionParameterType.TrackingHeader] = "",
            [SessionParametersMessage.SessionParameterType.TutorialEnabled] = false
        }));

        await ctx.Connection.SendMessageAsync(
            new AvailableFigurePartsMessage(ctx)
        );
    }
}
