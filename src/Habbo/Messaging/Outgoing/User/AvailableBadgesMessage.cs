using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;
using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class AvailableBadgesMessage : OutgoingMessage
{
    public AvailableBadgesMessage(List<string> badges, string currentBadge, bool showBadge) : base(229)
    {
        this.Build(badges, currentBadge, showBadge);
    }
    public AvailableBadgesMessage(IncomingMessageContext ctx) : base(229)
    {
        if(ctx.User is not Database.Models.User user || user is null)
        
            this.Build([], "", false);
        else
            this.Build(user.Badges, user?.CurrentBadge ?? "", user!.ShowBadge);
    }

    private void Build(List<string> badges, string currentBadge, bool showBadge)
    {
        this.AppendWired(badges.Count);
        
        int badgeSlot = 0;
        int slotCounter = 0;

        foreach(string badge in badges)
        {
            this.AppendString(badge);
            if(badge == currentBadge)
                badgeSlot = slotCounter;
            slotCounter++;
        }

        this.AppendWired(badgeSlot);
        this.AppendBoolean(showBadge);
    }
}

