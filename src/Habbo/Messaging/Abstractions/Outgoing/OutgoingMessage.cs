using System.Numerics;
using System.Text;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;
using Achilles.TCP.Data;

namespace Achilles.Habbo.Messaging.Abstractions.Outgoing;

public class OutgoingMessage
{
    public MessageHeader Header { get; }
    public string Content { get; set; } = "";

    public OutgoingMessage(MessageHeader header)
    {
        this.Header = header;
    }
    public OutgoingMessage(MessageHeader header, string content)
    {
        this.Header = header;
        this.Content = content;
    }

    public OutgoingMessage AppendBoolean(bool value)
    {
        if (value)
            this.Content += HabboEncoding.VL64.Encode(1);
        else
            this.Content += HabboEncoding.VL64.Encode(0);
        return this;
    }
    public OutgoingMessage AppendWired(object o)
    {
        try
        {
#pragma warning disable CS8604 // Possible null reference argument.
            this.Content += HabboEncoding.VL64.Encode(int.Parse(Convert.ToString(o)));
#pragma warning restore CS8604 // Possible null reference argument.
        }
        catch { }
        return this;
    }

    public OutgoingMessage AppendBase64(object o)
    {
        try
        {
#pragma warning disable CS8604 // Possible null reference argument.
            this.Content += HabboEncoding.Base64.Encode(int.Parse(Convert.ToString(o)));
#pragma warning restore CS8604 // Possible null reference argument.
        }
        catch { }
        return this;
    }
    public OutgoingMessage Append(string s)
    {
        this.Content += s;
        return this;
    }
    public OutgoingMessage Append(object o)
    {
        this.Content += o;
        return this;
    }
    public OutgoingMessage AppendString(object o)
    {
        this.Content += o.ToString() + Convert.ToChar(2);
        return this;
    }
    public OutgoingMessage AppendString(object o, int breaker)
    {
        this.Content += o.ToString() + Convert.ToChar(breaker);
        return this;
    }
    public OutgoingMessage AppendString(object o, char breaker)
    {
        this.Content += o.ToString() + breaker;
        return this;
    }
    public OutgoingMessage AppendChar(int c)
    {
        this.Content += Convert.ToChar(c);
        return this;
    }
    
    public Message ToMessage()
    {
        StringBuilder message = new();
        message.Append(this.Header.Encoded);
        message.Append(this.Content);
        message.Append(Convert.ToChar(1));

        return new Message(message.ToString());
    }

    public static implicit operator Message(OutgoingMessage message) => message.ToMessage();
}