using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Register;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Register;

[IncomingMessage(42)]
public class ApproveNameMessage : IncomingMessage
{
    public string Username { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ApproveNameMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.Username = content.ReadString();
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if(this.Username.Length < 1)
            return new ApproveNameResponseMessage(ApproveNameResponseMessage.ApproveNameResponseType.TooShort);
        
        if(this.Username.Length > 15)
            return new ApproveNameResponseMessage(ApproveNameResponseMessage.ApproveNameResponseType.TooLong);
        
        if(this.Username.Any(c => !char.IsLetterOrDigit(c)))
            return new ApproveNameResponseMessage(ApproveNameResponseMessage.ApproveNameResponseType.InvalidCharacters);
        
        bool usernameInUse = await ctx.Database.Users.AnyAsync(x => x.Username == this.Username);
        if(usernameInUse)
            return new ApproveNameResponseMessage(ApproveNameResponseMessage.ApproveNameResponseType.Taken);

        return new ApproveNameResponseMessage(ApproveNameResponseMessage.ApproveNameResponseType.Success);
    }
}
