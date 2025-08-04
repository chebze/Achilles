using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Register;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Incoming.Register;

[IncomingMessage(203)]
public class ApprovePasswordMessage : IncomingMessage
{
    public string Username { get; set; }
    public string Password { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ApprovePasswordMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.Username = content.ReadString();
        this.Password = content.ReadString();
    }

    public override OutgoingMessage? Respond(IncomingMessageContext ctx)
    {
        if(this.Password.Length < 6)
            return new ApprovePasswordResponseMessage(ApprovePasswordResponseMessage.ApprovePasswordResponseType.TooShort);
        
        if(this.Password.Length > 10)
            return new ApprovePasswordResponseMessage(ApprovePasswordResponseMessage.ApprovePasswordResponseType.TooLong);
                
        if(this.Password == this.Username)
            return new ApprovePasswordResponseMessage(ApprovePasswordResponseMessage.ApprovePasswordResponseType.SameAsUsername);

        return new ApprovePasswordResponseMessage(ApprovePasswordResponseMessage.ApprovePasswordResponseType.Success);
    }
}
