using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Alert;

public class LogoutMessage : OutgoingMessage
{
    public enum LogoutReason
    {
        Disconnect = -1,
        LoggedOut = 1,
        LogoutConcurrent = 2,
        LogoutTimeout = 3
    }

    public LogoutMessage(LogoutReason reason) : base(287)
    {
        this.AppendWired((int) reason);
    }
}