using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Purse;

public class VoucherRedeemErrorMessage : OutgoingMessage
{
    public enum VoucherRedeemErrorType
    {
        TechnicalError = 0,
        Invalid = 1,
        ProductDeliveryFailed = 2,
        WebOnly = 3
    }

    public VoucherRedeemErrorMessage(VoucherRedeemErrorType type) : base(213)
    {
        this.AppendWired((int) type);
    }
}

