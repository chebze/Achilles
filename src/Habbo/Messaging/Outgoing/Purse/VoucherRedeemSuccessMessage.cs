using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Purse;

public class VoucherRedeemSuccessMessage : OutgoingMessage
{
    public VoucherRedeemSuccessMessage() : base(212)
    {

    }
    
    public VoucherRedeemSuccessMessage(List<string> redeemedItems) : base(212)
    {
        if(redeemedItems.Count > 0)
        {
            foreach(var item in redeemedItems)
                this.AppendString(item);

            this.AppendString("");
        }
    }
}