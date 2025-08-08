using System.Reflection;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging;

public static class IncomingMessageTypeResolver
{
    private static readonly Dictionary<int, Type> _messageTypes = new();

    static IncomingMessageTypeResolver()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsSubclassOf(typeof(IncomingMessage)) && type.IsClass)
            {
                if(type.GetCustomAttribute<IncomingMessageAttribute>() is IncomingMessageAttribute attribute)
                {
                    if(_messageTypes.ContainsKey(attribute.Header))
                        throw new Exception($"Duplicate incoming message header {attribute.Header} from {type.Name}");

                    _messageTypes.Add(attribute.Header, type);
                }
            }
        }
    }

    public static void ReplaceResolver(int header, Type type)
    {
        _messageTypes[header] = type;
    }
    public static void ReplaceResolver<T>(int header)
        where T : IncomingMessage
    {
        _messageTypes[header] = typeof(T);
    }

    public static IEnumerable<IncomingMessage> Resolve(IMessage message)
    {
        string data = message.ToString();
        if(data.Length < 3)
            yield break;

        List<Exception> exceptions = [];

        while(data.Length > 0)
        {
            string encodedPacketLength = data.Substring(0, 3);
            int packetLength = HabboEncoding.Base64.Decode(encodedPacketLength);
            data = data.Substring(3);

            string packet = data.Substring(0, packetLength);
            data = data.Substring(packetLength);

            string header = packet.Substring(0, 2);
            int headerValue = HabboEncoding.Base64.Decode(header);
            packet = packet.Substring(2);

            if(!_messageTypes.TryGetValue(headerValue, out Type? messageType))
            {
                exceptions.Add(new Exception($"Unknown message header {headerValue}{Environment.NewLine}Packet: {packet}"));
                continue;
            }

            yield return (IncomingMessage) Activator.CreateInstance(
                messageType,
                new MessageHeader(headerValue),
                packet
            )!;
        }

        if(exceptions.Any())
            throw new AggregateException(exceptions);
    }
}
