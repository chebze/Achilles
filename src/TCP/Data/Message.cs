using Achilles.TCP.Abstractions;

namespace Achilles.TCP.Data;

public class Message : IMessage
{
    public string Content { get; set; }

    public Message(string content)
    {
        Content = content;
    }

    public override string ToString() => Content;
}