using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Achilles.Habbo.Messaging.Outgoing.Club;
using Achilles.Habbo.Messaging.Outgoing.Handshake;
using Achilles.Habbo.Messaging.Outgoing.Purse;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.TCP.Abstractions;
using static Achilles.Habbo.Messaging.Outgoing.Purse.SendCreditLogMessage;

namespace Achilles.Habbo.Messaging.Incoming.Club;

[IncomingMessage(190)]
public class PurchaseSubscriptionMessage : IncomingMessage
{
    public string SubscriptionName { get; set; }
    public int Choice { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public PurchaseSubscriptionMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.SubscriptionName = content.ReadString();
        this.Choice = content.ReadWiredInt();
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        var user = ctx.User;
        if (user == null)
            return null;

        int months = 0;
        int cost = 0;

        switch (this.Choice)
        {
            case 1:
                months = 1;
                cost = ctx.Configuration.Club.OneMonthCost;
                break;
            case 2:
                months = 3;
                cost = ctx.Configuration.Club.ThreeMonthsCost;
                break;
            case 3:
                months = 6;
                cost = ctx.Configuration.Club.SixMonthsCost;
                break;
            default:
                return null;
        }

        if(user.Credits < cost)
        {
            await ctx.Connection.SendMessageAsync(
                new NotEnoughCreditsMessage()
            );
            return null;
        }

        if(user.IsClubMember)
        {
            user.ClubSubscriptionEnd = user.ClubSubscriptionEnd!.Value.AddMonths(months);
            user.Credits -= cost;
        }
        else
        {
            user.ClubSubscriptionStart = DateTime.Now;
            user.ClubSubscriptionEnd = user.ClubSubscriptionStart.Value.AddMonths(months);
            user.Credits -= cost;
        }

        await ctx.Database.UserTransactions.AddAsync(new()
        {
            UserId = user.Id,
            TransactionSystemName = TransactionSystemNames.HabboClub,
            RealValue = 0,
            Currency = "",
            CreditValue = -cost,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Time = TimeOnly.FromDateTime(DateTime.Now)
        });
        ctx.Database.Users.Update(user);
        await ctx.Database.SaveChangesAsync();

        await ctx.Connection.SendMessageAsync(
            new SendCreditLogMessage(ctx)
        );
        await ctx.Connection.SendMessageAsync(
            new SendCreditsMessage(ctx)
        );
        await ctx.Connection.SendMessageAsync(
            new FuseRightsMessage(ctx)
        );
        await ctx.Connection.SendMessageAsync(
            new AvailableBadgesMessage(ctx)
        );
        await ctx.Connection.SendMessageAsync(
            new AvailableFigurePartsMessage(ctx)
        );
        
        return new ClubInfoMessage(ctx);
    }
}