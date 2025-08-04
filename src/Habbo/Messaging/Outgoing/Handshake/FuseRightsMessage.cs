using Achilles.Habbo.Configuration;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.Handshake;

public class FuseRightsMessage : OutgoingMessage
{
    public FuseRightsMessage(List<string> rights) : base(2)
    {
        foreach(string right in rights)
            this.AppendString(right);
    }
    public FuseRightsMessage(IncomingMessageContext ctx) : base(2)
    {
        List<string> rights = [];
        HabboConfiguration configuration = ctx.Configuration;

        if(ctx.User is var user && user is not null)
        {
            rights.AddRange(
                configuration.Ranks
                    .FirstOrDefault(r => r.Name == configuration.DefaultRankName)?
                    .FuseRights.ToList() ?? []
            );
            rights.AddRange(
                configuration.Ranks
                    .FirstOrDefault(r => r.Name == user.Rank)?
                    .FuseRights.ToList() ?? []
            );

            if(user.ClubSubscriptionStart is not null && user.ClubSubscriptionEnd is not null)
            {
                if(user.ClubSubscriptionStart <= DateTime.Now && user.ClubSubscriptionEnd >= DateTime.Now)
                {
                    rights.AddRange([
                        "fuse_priority_access",
                        "fuse_use_special_room_layouts",
                        "fuse_use_club_outfits",
                        "fuse_use_club_outfits_default",
                        "fuse_use_club_badge",
                        "fuse_use_club_dance",
                        "fuse_habbo_chooser",
                        "fuse_furni_chooser",
                        "fuse_extended_buddylist",
                        "fuse_room_queue_club"
                    ]);
                }
            }
        }

        foreach(string right in rights)
            this.AppendString(right);
    }
}