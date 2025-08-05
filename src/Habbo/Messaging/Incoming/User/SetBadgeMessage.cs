using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;

namespace Achilles.Habbo.Messaging.Incoming.User;

[IncomingMessage(158)]
public class SetBadgeMessage : IncomingMessage
{
    public string Badge { get; set; }
    public bool ShowBadge { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SetBadgeMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        content.ReadWiredInt();
        this.Badge = content.ReadString();
        
    }
}
