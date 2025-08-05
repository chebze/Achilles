using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Messenger;

public class MessageReceivedMessage : OutgoingMessage
{
    public MessageReceivedMessage(
        int messageId,
        int fromUserId,
        string dateString,
        string message
    ) : base(134)
    {
        this.AppendWired(1);
        this.AppendWired(messageId);
        this.AppendWired(fromUserId);
        this.AppendString(dateString);
        this.AppendString(message);
    }
}
