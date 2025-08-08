using System.Runtime.CompilerServices;
using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Club;
using Achilles.Habbo.Messaging.Outgoing.Handshake;
using Achilles.Habbo.Messaging.Outgoing.Purse;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;
using static Achilles.Habbo.Messaging.Outgoing.Purse.SendCreditLogMessage;

namespace Achilles.Habbo.Messaging.Incoming.Purse;

[IncomingMessage(129)]
public class RedeemVoucherMessage : IncomingMessage
{
    public string Code { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public RedeemVoucherMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.Code = content.ReadString();
    }

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        var user = ctx.User;
        if (user is null)
            return;

        Voucher? voucher = await ctx.Database.Vouchers.FirstOrDefaultAsync(v => v.Code == this.Code);

        if (voucher is null)
        {
            await ctx.Connection.SendMessageAsync(
                new VoucherRedeemErrorMessage(VoucherRedeemErrorMessage.VoucherRedeemErrorType.Invalid)
            );
            return;
        }

        switch (voucher.Type)
        {
            case VoucherType.Credits:
                user.Credits += voucher.Value;
                await ctx.Connection.SendMessageAsync(
                    new VoucherRedeemSuccessMessage()
                );
                    ctx.Database.UserTransactions.Add(new()
                    {
                        UserId = user.Id,
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        Time = TimeOnly.FromDateTime(DateTime.Now),
                        CreditValue = voucher.Value,
                        RealValue = 0,
                        Currency = "",
                        TransactionSystemName = TransactionSystemNames.Voucher
                    });
                    await ctx.Database.SaveChangesAsync();
                await ctx.Connection.SendMessageAsync(
                    new SendCreditsMessage(ctx)
                );
                break;
            case VoucherType.Tickets:
                user.Tickets += voucher.Value;
                await ctx.Connection.SendMessageAsync(
                    new VoucherRedeemSuccessMessage([
                        $"{voucher.Value} tickets"
                    ])
                );
                await ctx.Connection.SendMessageAsync(
                    new SendUserInfoMessage(ctx)
                );
                break;
            case VoucherType.Films:
                user.Film += voucher.Value;
                await ctx.Connection.SendMessageAsync(
                    new VoucherRedeemSuccessMessage([
                        $"{voucher.Value} films"
                    ])
                );
                await ctx.Connection.SendMessageAsync(
                    new SendUserInfoMessage(ctx)
                );
                break;
            case VoucherType.HabboClubMonths:
                if(!user.IsClubMember)
                {
                    user.ClubSubscriptionStart = DateTime.Now;
                    user.ClubSubscriptionEnd = DateTime.Now.AddMonths(voucher.Value);
                }
                else
                {
                    user.ClubSubscriptionEnd = user.ClubSubscriptionEnd!.Value.AddMonths(voucher.Value);
                }
                await ctx.Connection.SendMessageAsync(
                    new VoucherRedeemSuccessMessage([
                        $"{voucher.Value} months of Habbo Club"
                    ])
                );
                await ctx.Connection.SendMessageAsync(
                    new FuseRightsMessage(await UserUtilities.GetFuseRights(ctx))
                );
                await ctx.Connection.SendMessageAsync(
                    new AvailableBadgesMessage(ctx)
                );
                await ctx.Connection.SendMessageAsync(
                    new AvailableFigurePartsMessage(ctx)
                );
                break;
            case VoucherType.Furniture:
                await ctx.Connection.SendMessageAsync(
                    new VoucherRedeemErrorMessage(
                        VoucherRedeemErrorMessage.VoucherRedeemErrorType.ProductDeliveryFailed
                    )
                );
                break;
        }
        
        await ctx.Connection.SendMessageAsync(
            new SendCreditLogMessage(ctx)
        );

        ctx.Database.Users.Update(user);
        ctx.Database.Vouchers.Remove(voucher);
        await ctx.Database.SaveChangesAsync();
    }
    
}
