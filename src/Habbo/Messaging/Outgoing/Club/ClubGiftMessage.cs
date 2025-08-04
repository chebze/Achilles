using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Club;

public class ClubGiftMessage : OutgoingMessage
{
    public ClubGiftMessage(string giftCount) : base(280)
    {
        this.AppendWired(giftCount);
    }
}