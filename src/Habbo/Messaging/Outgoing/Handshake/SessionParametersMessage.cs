using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Handshake;

public class SessionParametersMessage : OutgoingMessage
{
    public enum SessionParameterType
    {
        EnableCOPPA = 0,
        EnableVouchers = 1,
        RegisterRequireParentEmail = 2,
        RegisterSendParentEmail = 3,
        AllowDirectMail = 4,
        DateFormat = 5,
        PartnerIntegrationEnabled = 6,
        AllowProfileEditing = 7,
        TrackingHeader = 8,
        TutorialEnabled = 9
    }

    public SessionParametersMessage(Dictionary<SessionParameterType, object> parameters) : base(257)
    {
        this.AppendWired(parameters.Count);
        foreach(var parameter in parameters)
        {
            this.AppendWired((int) parameter.Key);

            if(parameter.Value is bool boolValue)
                this.AppendBoolean(boolValue);
            
            else if(parameter.Value is string stringValue)
                this.AppendString(stringValue);

            else if(parameter.Value is int intValue)
                this.AppendWired(intValue);
        }
    }
}

