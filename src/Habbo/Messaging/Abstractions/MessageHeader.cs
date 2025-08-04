using Achilles.Habbo.Utilities;

namespace Achilles.Habbo.Messaging.Abstractions;

public class MessageHeader
{
    public int Value { get; }
    public string Encoded => HabboEncoding.Base64.Encode(Value);

    public MessageHeader(int value)
    {
        Value = value;
    }

    public static implicit operator int(MessageHeader header) => header.Value;
    public static implicit operator string(MessageHeader header) => header.Encoded;
    public static implicit operator MessageHeader(int value) => new(value);
    public static implicit operator MessageHeader(string value) => new(HabboEncoding.Base64.Decode(value));
}