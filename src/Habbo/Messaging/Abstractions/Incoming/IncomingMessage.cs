using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Abstractions.Incoming;

public abstract class IncomingMessage
{
    public MessageHeader Header { get; }
    public string Raw { get; }

    public IncomingMessage(MessageHeader header, string raw)
    {
        this.Header = header;
        this.Raw = raw;
        this.Parse(new IncomingMessageContent(raw));
    }
    public IncomingMessage(int header, string raw)
    {
        this.Header = new MessageHeader(header);
        this.Raw = raw;
        this.Parse(new IncomingMessageContent(raw));
    }

    protected abstract void Parse(IncomingMessageContent content);

    public virtual void Handle(IncomingMessageContext ctx)
    {
    }
    public virtual async Task HandleAsync(IncomingMessageContext ctx)
    {
        await Task.CompletedTask;
    }

    public virtual OutgoingMessage? Respond(IncomingMessageContext ctx)
    {
        return null;
    }
    public virtual async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        return await Task.FromResult<OutgoingMessage?>(null);
    }
}